using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker_EnterpriseEdition.Application.Abstractions;
using PersonalFinanceTracker_EnterpriseEdition.Application.DTOs.Transactions;
using PersonalFinanceTracker_EnterpriseEdition.Domain.Configurations;
using PersonalFinanceTracker_EnterpriseEdition.Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;
using PersonalFinanceTracker_EnterpriseEdition.Application.Extensions;
using PersonalFinanceTracker_EnterpriseEdition.Domain.Exceptions;
using PersonalFinanceTracker_EnterpriseEdition.Application.DTOs.Categories;
using PersonalFinanceTracker_EnterpriseEdition.Domain.Enums;

namespace PersonalFinanceTracker_EnterpriseEdition.Application.Services;

public class TransactionService(IRepository<Transaction> transactionRepository,
                                IRepository<Category> categoryRepository,
                                IAuditLogService auditLogService,
                                ICacheService cacheService) : ITransactionService
{
    private readonly IRepository<Transaction> _transactionRepository = transactionRepository;
    private readonly IRepository<Category> _categoryRepository = categoryRepository;
    private readonly IAuditLogService _auditLogService = auditLogService;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<GetTransactionDto> CreateAsync(Guid userId,CreateTransactionDto dto)
    {
        var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
        if (category is null || category.UserId != userId)
            throw new CustomException(404, "Category not found");
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
        return new GetTransactionDto
        {
            Id = transaction.Id,
            Amount = transaction.Amount,
            Type = transaction.Type,
            Note = transaction.Note,
            CreatedAt = transaction.CreatedAt,
            Category = new GetCategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Color = category.Color
            }
        };
    }

    public async Task<GetTransactionDto> UpdateAsync(Guid userId,Guid id, UpdateTransactionDto dto)
    {
        var transaction = await _transactionRepository.GetByIdAsync(id);
        if (transaction == null || transaction.UserId != userId) throw new CustomException(404,"Transaction not found");
        
        // Null check for RowVersion
        if (transaction.RowVersion != null && dto.RowVersion != null && !transaction.RowVersion.SequenceEqual(dto.RowVersion))
            throw new CustomException(409,"Transaction has been modified by another process (concurrency conflict)");
            
        var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
        if (category is null || category.UserId != userId)
            throw new CustomException(404, "Category not found");

        var oldValue = new { transaction.Amount, transaction.Type, transaction.CategoryId, transaction.Note };
        transaction.Amount = dto.Amount;
        transaction.Type = dto.Type;
        transaction.CategoryId = dto.CategoryId;
        transaction.Note = dto.Note;
        transaction.RowVersion = dto.RowVersion;
        await _transactionRepository.UpdateAsync(transaction);
        await _transactionRepository.SaveChangesAsync();
        await _auditLogService.LogUpdateAsync(nameof(Transaction), transaction.Id, userId, oldValue, transaction);

        return new GetTransactionDto
        {
            Id = transaction.Id,
            Amount = transaction.Amount,
            Type = transaction.Type,
            Note = transaction.Note,
            CreatedAt = transaction.CreatedAt,
            Category = new GetCategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Color = category.Color
            }
        };
    }

    public async Task<bool> DeleteAsync(Guid userId,Guid id)
    {
        var transaction = await _transactionRepository.GetByIdAsync(id);
        if (transaction == null || transaction.UserId != userId) return false;
        var oldValue = new { transaction.Amount, transaction.Type, transaction.CategoryId, transaction.Note };
        await _transactionRepository.DeleteAsync(id);
        await _transactionRepository.SaveChangesAsync();
        await _auditLogService.LogDeleteAsync(nameof(Transaction), transaction.Id, userId, oldValue);
        return true;
    }

    public async Task<GetTransactionDto> GetByIdAsync(Guid userId,Guid id)
    {
        var transaction = await _transactionRepository.GetByIdAsync(id, ["Category"]);
        if (transaction == null || transaction.UserId != userId) return null;
        var category = transaction.Category;
        return new GetTransactionDto
        {
            Id = transaction.Id,
            Amount = transaction.Amount,
            Type = transaction.Type,
            Note = transaction.Note,
            CreatedAt = transaction.CreatedAt,
            Category = category == null ? null : new DTOs.Categories.GetCategoryDto
            {
                Id = category.Id,
                Color = category.Color,
                Name = category.Name,
            }
        };
    }

    public async Task<List<GetTransactionDto>> GetAllAsync(Guid userId,PaginationParams @params, string sort = null, string filter = null)
    {
        var query = _transactionRepository.Query(t => t.UserId == userId)
                                                             .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filter))
            query = query.Where(t => t.Note != null && t.Note.Contains(filter));

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

        return await query.ToPagedList(@params)
                          .Select(t => new GetTransactionDto
                          {
                              Id = t.Id,
                              Amount = t.Amount,
                              Type = t.Type,
                              Note = t.Note,
                              CreatedAt = t.CreatedAt,
                              Category = t.Category == null ? null :new DTOs.Categories.GetCategoryDto
                              {
                                  Id = t.CategoryId,
                                  Color = t.Category.Color,
                                  Name = t.Category.Name,
                              }
                          })
                          .ToListAsync();
    }

    public async Task<TransactionSummaryDto> GetMonthlySummaryAsync(Guid userId, int year, int month)
    {
        var cacheKey = $"summary:{userId}:{year}:{month}";
        var cachedBytes = await _cacheService.GetCacheAsync<TransactionSummaryDto>(cacheKey);
        if (cachedBytes != null)
            return cachedBytes;
        var query = _transactionRepository.Query(t => t.UserId == userId && t.CreatedAt.Year == year && t.CreatedAt.Month == month);
        var totalIncome = await query.Where(t => t.Type == TransactionType.Income).SumAsync(t => t.Amount);
        var totalExpense = await query.Where(t => t.Type == TransactionType.Expense).SumAsync(t => t.Amount);
        var result = new TransactionSummaryDto
        {
            TotalIncome = totalIncome,
            TotalExpense = totalExpense
        };
        await _cacheService.CreateCacheAsync(cacheKey,result);
        return result;
    }

    public async Task<List<CategoryExpenseStatDto>> GetTopCategoryExpensesAsync(Guid userId, int year, int month, int top = 3)
    {
        var cacheKey = $"topcat:{userId}:{year}:{month}:{top}";
        var cachedBytes = await _cacheService.GetCacheAsync<List<CategoryExpenseStatDto>>(cacheKey);
        if (cachedBytes != null)
            return cachedBytes;
        var query = _transactionRepository.Query(t => t.UserId == userId && t.CreatedAt.Year == year && t.CreatedAt.Month == month && t.Type == TransactionType.Expense);
        var stats = await query.GroupBy(t => t.CategoryId)
            .Select(g => new { CategoryId = g.Key, TotalExpense = g.Sum(t => t.Amount) })
            .OrderByDescending(x => x.TotalExpense)
            .Take(top)
            .ToListAsync();

        var categoryIds = stats.Select(s => s.CategoryId).ToList();
        var categories = await _categoryRepository.Query(c => categoryIds.Contains(c.Id))
                                                              .ToListAsync();
        var result = categories.Select(c => new CategoryExpenseStatDto
        {
            Id = c.Id,
            Name = c.Name,
            TotalExpense = stats.FirstOrDefault(stat => stat.CategoryId == c.Id)?.TotalExpense ?? 0
        }).OrderByDescending(s =>s.TotalExpense).ToList();

        await _cacheService.CreateCacheAsync(cacheKey, result);
        return result;
    }

    public async Task<List<MonthlyTrendDto>> GetMonthlyTrendAsync(Guid userId,int monthsCount = 6)
    {
        var cacheKey = $"trend:{userId}:{monthsCount}";
        var cachedBytes = await _cacheService.GetCacheAsync<List<MonthlyTrendDto>>(cacheKey);
        if (cachedBytes != null)
            return cachedBytes;
        var fromDate = DateTime.UtcNow.AddMonths(-monthsCount + 1);
        var query = _transactionRepository.Query(t => t.UserId == userId && t.CreatedAt >= fromDate);
        var trends = await query.GroupBy(t => new { t.CreatedAt.Year, t.CreatedAt.Month })
            .Select(g => new MonthlyTrendDto
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Income = g.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount),
                Expense = g.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount)
            })
            .OrderBy(x => x.Year)
            .ThenBy(x => x.Month)
            .ToListAsync();
        await _cacheService.CreateCacheAsync(cacheKey,trends);
        return trends;
    }
} 