using PersonalFinanceTracker_EnterpriseEdition.Application.DTOs.Users;
using PersonalFinanceTracker_EnterpriseEdition.Domain.Configurations;

namespace PersonalFinanceTracker_EnterpriseEdition.Application.Abstractions;

public interface IUserService
{
    Task<List<GetUserDto>> GetAllAsync(string search, PaginationParams @params);
    Task<GetUserDto> GetByIdAsync(Guid id);
    Task<GetUserForMeDto> GetForMeAsync(Guid userId);
    Task<GetUserForMeDto> UpdateMeAsync(Guid userId, UpdateUserDto dto);
    Task<bool> DeleteAsync(Guid id);
} 