using PersonalFinanceTracker_EnterpriseEdition.Domain.Commons;

namespace PersonalFinanceTracker_EnterpriseEdition.Domain.Entities;

public class Category : Auditable
{
    public string Name { get; set; } = default!;
    public string Color { get; set; } = default!;
    public Guid UserId { get; set; }
    public User User { get; set; } = default!;
} 