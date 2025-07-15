namespace PersonalFinanceTracker_EnterpriseEdition.Application.DTOs.Users;

public record GetUserForMeDto
{
    public Guid Id { get; init; }
    public string Username { get; init; }
    public string Email { get; init; }
    public DateTime CreatedAt { get; init; }
}