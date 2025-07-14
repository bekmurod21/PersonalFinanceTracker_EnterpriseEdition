using PersonalFinanceTracker_EnterpriseEdition.Application.DTOs.Transactions;
using PersonalFinanceTracker_EnterpriseEdition.Domain.Configurations;

namespace PersonalFinanceTracker_EnterpriseEdition.Application.Abstractions;

public interface ITransactionService
{
    Task<GetTransactionDto> CreateAsync(CreateTransactionDto dto);
    Task<GetTransactionDto> UpdateAsync(Guid id, UpdateTransactionDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task<GetTransactionDto> GetByIdAsync(Guid id);
    Task<List<GetTransactionDto>> GetAllAsync(PaginationParams @params, string sort = null, string filter = null);
    Task<TransactionSummaryDto> GetMonthlySummaryAsync( int year, int month);
    Task<List<CategoryExpenseStatDto>> GetTopCategoryExpensesAsync(int year, int month, int top = 3);
    Task<List<MonthlyTrendDto>> GetMonthlyTrendAsync(int monthsCount = 6);
} 