using PersonalFinanceTracker_EnterpriseEdition.Domain.Enums;

namespace PersonalFinanceTracker_EnterpriseEdition.Application.DTOs.Users;

public record GetUserDto
{
    public Guid Id { get; init; }
    public string Username { get; init; }
    public string Email { get; init; }
    public DateTime CreatedAt { get; init; }
    public ERole Role { get; init; }
}