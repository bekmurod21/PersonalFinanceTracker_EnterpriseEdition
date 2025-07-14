namespace PersonalFinanceTracker_EnterpriseEdition.Application.DTOs.Users;

public class SignInDto
{
    public string EmailOrUserName { get; set; } = default!;
    public string Password { get; set; } = default!;
} 