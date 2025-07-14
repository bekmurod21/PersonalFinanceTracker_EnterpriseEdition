using PersonalFinanceTracker_EnterpriseEdition.Application.DTOs.Transactions;
using PersonalFinanceTracker_EnterpriseEdition.Domain.Configurations;

namespace PersonalFinanceTracker_EnterpriseEdition.Application.Abstractions;

public interface ITransactionService
{
    Task<GetTransactionDto> CreateAsync(CreateTransactionDto dto, Guid userId);
    Task<GetTransactionDto> UpdateAsync(Guid id, UpdateTransactionDto dto, Guid userId);
    Task<bool> DeleteAsync(Guid id, Guid userId);
    Task<GetTransactionDto> GetByIdAsync(Guid id, Guid userId);
    Task<List<GetTransactionDto>> GetAllAsync(Guid userId, PaginationParams @params, string sort = null, string filter = null);
    Task<TransactionSummaryDto> GetMonthlySummaryAsync(Guid userId, int year, int month);
    Task<List<CategoryExpenseStatDto>> GetTopCategoryExpensesAsync(Guid userId, int year, int month, int top = 3);
    Task<List<MonthlyTrendDto>> GetMonthlyTrendAsync(Guid userId, int monthsCount = 6);
} 