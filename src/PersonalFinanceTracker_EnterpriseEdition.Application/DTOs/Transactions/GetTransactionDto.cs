using PersonalFinanceTracker_EnterpriseEdition.Application.DTOs.Categories;
using PersonalFinanceTracker_EnterpriseEdition.Domain.Enums;

namespace PersonalFinanceTracker_EnterpriseEdition.Application.DTOs.Transactions;

public class GetTransactionDto
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public GetCategoryDto Category { get; set; }
    public string Note { get; set; }
    public DateTime CreatedAt { get; set; }
} 