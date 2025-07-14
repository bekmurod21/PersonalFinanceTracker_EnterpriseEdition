using PersonalFinanceTracker_EnterpriseEdition.Domain.Commons;
using PersonalFinanceTracker_EnterpriseEdition.Domain.Enums;

namespace PersonalFinanceTracker_EnterpriseEdition.Domain.Entities;

public class Transaction : Auditable
{
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = default!;
    public Guid UserId { get; set; }
    public User User { get; set; } = default!;
    public string? Note { get; set; }
    public byte[] RowVersion { get; set; } = default!;
} 