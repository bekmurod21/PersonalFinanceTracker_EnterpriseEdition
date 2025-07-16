namespace PersonalFinanceTracker_EnterpriseEdition.Domain.Commons;

public class Auditable
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; }
}