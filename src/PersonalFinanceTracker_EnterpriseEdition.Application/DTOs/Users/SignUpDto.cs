using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceTracker_EnterpriseEdition.Application.DTOs.Users;

public class SignUpDto
{
    [EmailAddress]
    public string Email { get; set; } = default!;
    public string Username { get; set; } = default!;
    public string Password { get; set; } = default!;
} 