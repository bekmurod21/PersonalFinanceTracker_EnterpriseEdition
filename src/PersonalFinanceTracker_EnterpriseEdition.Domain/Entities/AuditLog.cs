using PersonalFinanceTracker_EnterpriseEdition.Domain.Commons;

namespace PersonalFinanceTracker_EnterpriseEdition.Domain.Entities;

public class AuditLog : Auditable
{
    public Guid UserId { get; set; }
    public string Action { get; set; } = default!;
    public string EntityName { get; set; } = default!;
    public Guid EntityId { get; set; }
    public string OldValue { get; set; }
    public string NewValue { get; set; }
} 