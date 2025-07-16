using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceTracker_EnterpriseEdition.Application.DTOs.Users;

public record UpdateUserDto
{
    public string Username { get; set; }
    [EmailAddress]
    public string Email { get; set; }
    public string Password { get; set; }
}
