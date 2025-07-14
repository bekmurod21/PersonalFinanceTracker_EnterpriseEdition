using PersonalFinanceTracker_EnterpriseEdition.Domain.Entities;
using PersonalFinanceTracker_EnterpriseEdition.Application.DTOs.Users;

namespace PersonalFinanceTracker_EnterpriseEdition.Application.Abstractions;

public interface IAuthService
{
    string GenerateJwtToken(User user);
    Task<User> SignUpAsync(SignUpDto dto);
    Task<(string AccessToken, string RefreshToken)> SignInAsync(SignInDto dto);
}