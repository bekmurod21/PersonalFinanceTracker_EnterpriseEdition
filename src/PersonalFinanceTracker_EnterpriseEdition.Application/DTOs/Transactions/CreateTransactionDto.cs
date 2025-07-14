using PersonalFinanceTracker_EnterpriseEdition.Domain.Enums;

namespace PersonalFinanceTracker_EnterpriseEdition.Application.DTOs.Transactions;

public class CreateTransactionDto
{
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public Guid CategoryId { get; set; }
    public string? Note { get; set; }
} 