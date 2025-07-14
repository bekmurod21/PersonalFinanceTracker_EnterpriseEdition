using PersonalFinanceTracker_EnterpriseEdition.Domain.Enums;

namespace PersonalFinanceTracker_EnterpriseEdition.Application.DTOs.Users;

public record GetUserDto
{
    public Guid Id { get; init; }
    public string UserName { get; init; }
    public string Email { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public DateTime CreatedAt { get; init; }
    public ERole Role { get; init; }
}