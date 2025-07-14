using System.ComponentModel.DataAnnotations;
using PersonalFinanceTracker_EnterpriseEdition.Domain.Commons;
using PersonalFinanceTracker_EnterpriseEdition.Domain.Enums;

namespace PersonalFinanceTracker_EnterpriseEdition.Domain.Entities;

public class User : Auditable
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    [EmailAddress]
    public string Email { get; set; }
    public string Password { get; set; }
    public string UserName { get; set; }
    public ERole Role { get; set; } = ERole.User;
}