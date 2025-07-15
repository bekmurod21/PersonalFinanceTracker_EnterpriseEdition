namespace PersonalFinanceTracker_EnterpriseEdition.Application.DTOs.Users;

public record UpdateUserDto
{
    public string Username { get; set; }
    public string Email { get; set; }
}
