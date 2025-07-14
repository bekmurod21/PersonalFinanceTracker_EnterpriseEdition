using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker_EnterpriseEdition.Application.Abstractions;
using PersonalFinanceTracker_EnterpriseEdition.Application.DTOs.Transactions;
using PersonalFinanceTracker_EnterpriseEdition.Domain.Configurations;
using PersonalFinanceTracker_EnterpriseEdition.Domain.Entities;
using System.Linq.Expressions;
using Microsoft.Extensions.Caching.Memory;
using PersonalFinanceTracker_EnterpriseEdition.Application.Extensions;

namespace PersonalFinanceTracker_EnterpriseEdition.Application.Services;

public class TransactionService(IRepository<Transaction> transactionRepository,
                                IRepository<Category> categoryRepository,
                                IAuditLogService auditLogService,
                                IMemoryCache cache) : ITransactionService
{
    private readonly IRepository<Transaction> _transactionRepository = transactionRepository;
    private readonly IRepository<Category> _categoryRepository = categoryRepository;
    private readonly IAuditLogService _auditLogService = auditLogService;
    private readonly IMemoryCache _cache = cache;

    public async Task<GetTransactionDto> CreateAsync(CreateTransactionDto dto, Guid userId)
    {
        var transaction = new Transaction
        {
            Amount = dto.Amount,
            Type = dto.Type,
            CategoryId = dto.CategoryId,
            UserId = userId,
            Note = dto.Note
        };
        await _transactionRepository.AddAsync(transaction);
        await _transactionRepository.SaveChangesAsync();
        await _auditLogService.LogCreateAsync(nameof(Transaction), transaction.Id, userId, transaction);
        var category = await _categoryRepository.GetByIdAsync(transaction.CategoryId);
        return new GetTransactionDto
        {
            Id = transaction.Id,
            Amount = transaction.Amount,
            Type = transaction.Type,
            CategoryId = transaction.CategoryId,
            CategoryName = category?.Name ?? string.Empty,
            Note = transaction.Note,
            CreatedAt = transaction.CreatedAt
        };
    }

    public async Task<GetTransactionDto> UpdateAsync(Guid id, UpdateTransactionDto dto, Guid userId)
    {
        var transaction = await _transactionRepository.GetByIdAsync(id);
        if (transaction == null || transaction.UserId != userId) throw new Exception("Transaction not found");
        if (!transaction.RowVersion.SequenceEqual(dto.RowVersion))
            throw new Exception("Transaction has been modified by another process (concurrency conflict)");
        var oldValue = new { transaction.Amount, transaction.Type, transaction.CategoryId, transaction.Note };
        transaction.Amount = dto.Amount;
        transaction.Type = dto.Type;
        transaction.CategoryId = dto.CategoryId;
        transaction.Note = dto.Note;
        transaction.RowVersion = dto.RowVersion;
        await _transactionRepository.UpdateAsync(transaction);
        await _transactionRepository.SaveChangesAsync();
        await _auditLogService.LogUpdateAsync(nameof(Transaction), transaction.Id, userId, oldValue, transaction);
        var category = await _categoryRepository.GetByIdAsync(transaction.CategoryId);
        return new GetTransactionDto
        {
            Id = transaction.Id,
            Amount = transaction.Amount,
            Type = transaction.Type,
            CategoryId = transaction.CategoryId,
            CategoryName = category?.Name ?? string.Empty,
            Note = transaction.Note,
            CreatedAt = transaction.CreatedAt
        };
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId)
    {
        var transaction = await _transactionRepository.GetByIdAsync(id);
        if (transaction == null || transaction.UserId != userId) return false;
        var oldValue = new { transaction.Amount, transaction.Type, transaction.CategoryId, transaction.Note };
        await _transactionRepository.DeleteAsync(id);
        await _transactionRepository.SaveChangesAsync();
        await _auditLogService.LogDeleteAsync(nameof(Transaction), transaction.Id, userId, oldValue);
        return true;
    }

    public async Task<GetTransactionDto> GetByIdAsync(Guid id, Guid userId)
    {
        var transaction = await _transactionRepository.GetByIdAsync(id);
        if (transaction == null || transaction.UserId != userId) return null;
        var category = await _categoryRepository.GetByIdAsync(transaction.CategoryId);
        return new GetTransactionDto
        {
            Id = transaction.Id,
            Amount = transaction.Amount,
            Type = transaction.Type,
            CategoryId = transaction.CategoryId,
            CategoryName = category?.Name ?? string.Empty,
            Note = transaction.Note,
            CreatedAt = transaction.CreatedAt
        };
    }

    public async Task<List<GetTransactionDto>> GetAllAsync(
        Guid userId, PaginationParams @params, string sort = null, string filter = null)
    {
        var query = _transactionRepository.Query(t => t.UserId == userId);

        // Filter
        if (!string.IsNullOrWhiteSpace(filter))
        {
            query = query.Where(t => t.Note != null && t.Note.Contains(filter));
        }

        // Sort
        if (!string.IsNullOrWhiteSpace(sort))
        {
            if (sort.Equals("amount", StringComparison.OrdinalIgnoreCase))
                query = query.OrderByDescending(t => t.Amount);
            else if (sort.Equals("createdAt", StringComparison.OrdinalIgnoreCase))
                query = query.OrderByDescending(t => t.CreatedAt);
        }
        else
        {
            query = query.OrderByDescending(t => t.CreatedAt);
        }

        var items = await query.ToPagedList(@params)
                              .ToListAsync();

        var categoryIds = items.Select(x => x.CategoryId).Distinct().ToList();
        var categories = await _categoryRepository.GetAllAsync(c => categoryIds.Contains(c.Id));
        var result = items.Select(t => new GetTransactionDto
        {
            Id = t.Id,
            Amount = t.Amount,
            Type = t.Type,
            CategoryId = t.CategoryId,
            CategoryName = categories.FirstOrDefault(c => c.Id == t.CategoryId)?.Name ?? string.Empty,
            Note = t.Note,
            CreatedAt = t.CreatedAt
        }).ToList();

        return result;
    }

    public async Task<TransactionSummaryDto> GetMonthlySummaryAsync(Guid userId, int year, int month)
    {
        var cacheKey = $"summary:{userId}:{year}:{month}";
        if (_cache.TryGetValue(cacheKey, out TransactionSummaryDto cached))
            return cached!;
        var query = _transactionRepository.Query(t => t.UserId == userId && t.CreatedAt.Year == year && t.CreatedAt.Month == month);
        var totalIncome = await query.Where(t => t.Type == Domain.Enums.TransactionType.Income).SumAsync(t => t.Amount);
        var totalExpense = await query.Where(t => t.Type == Domain.Enums.TransactionType.Expense).SumAsync(t => t.Amount);
        var result = new TransactionSummaryDto
        {
            TotalIncome = totalIncome,
            TotalExpense = totalExpense
        };
        _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));
        return result;
    }

    public async Task<List<CategoryExpenseStatDto>> GetTopCategoryExpensesAsync(Guid userId, int year, int month, int top = 3)
    {
        var cacheKey = $"topcat:{userId}:{year}:{month}:{top}";
        if (_cache.TryGetValue(cacheKey, out List<CategoryExpenseStatDto> cached))
            return cached!;
        var query = _transactionRepository.Query(t => t.UserId == userId && t.CreatedAt.Year == year && t.CreatedAt.Month == month && t.Type == Domain.Enums.TransactionType.Expense);
        var stats = await query.GroupBy(t => t.CategoryId)
            .Select(g => new { CategoryId = g.Key, TotalExpense = g.Sum(t => t.Amount) })
            .OrderByDescending(x => x.TotalExpense)
            .Take(top)
            .ToListAsync();
        var categories = await _categoryRepository.GetAllAsync(c => stats.Select(s => s.CategoryId).Contains(c.Id));
        var result = stats.Select(s => new CategoryExpenseStatDto
        {
            CategoryId = s.CategoryId,
            CategoryName = categories.FirstOrDefault(c => c.Id == s.CategoryId)?.Name ?? string.Empty,
            TotalExpense = s.TotalExpense
        }).ToList();
        _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));
        return result;
    }

    public async Task<List<MonthlyTrendDto>> GetMonthlyTrendAsync(Guid userId, int monthsCount = 6)
    {
        var cacheKey = $"trend:{userId}:{monthsCount}";
        if (_cache.TryGetValue(cacheKey, out List<MonthlyTrendDto> cached))
            return cached!;
        var fromDate = DateTime.UtcNow.AddMonths(-monthsCount + 1);
        var query = _transactionRepository.Query(t => t.UserId == userId && t.CreatedAt >= fromDate);
        var data = await query.ToListAsync();
        var trends = data.GroupBy(t => new { t.CreatedAt.Year, t.CreatedAt.Month })
            .Select(g => new MonthlyTrendDto
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Income = g.Where(t => t.Type == Domain.Enums.TransactionType.Income).Sum(t => t.Amount),
                Expense = g.Where(t => t.Type == Domain.Enums.TransactionType.Expense).Sum(t => t.Amount)
            })
            .OrderBy(x => x.Year).ThenBy(x => x.Month)
            .ToList();
        _cache.Set(cacheKey, trends, TimeSpan.FromMinutes(5));
        return trends;
    }
} 