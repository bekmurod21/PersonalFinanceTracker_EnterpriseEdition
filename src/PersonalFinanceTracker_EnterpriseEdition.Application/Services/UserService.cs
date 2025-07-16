using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker_EnterpriseEdition.Application.Abstractions;
using PersonalFinanceTracker_EnterpriseEdition.Application.DTOs.Users;
using PersonalFinanceTracker_EnterpriseEdition.Application.Extensions;
using PersonalFinanceTracker_EnterpriseEdition.Application.Helpers;
using PersonalFinanceTracker_EnterpriseEdition.Domain.Configurations;
using PersonalFinanceTracker_EnterpriseEdition.Domain.Entities;
using PersonalFinanceTracker_EnterpriseEdition.Domain.Exceptions;

namespace PersonalFinanceTracker_EnterpriseEdition.Application.Services;

public class UserService(IRepository<User> userRepository, IAuditLogService auditLogService) : IUserService
{
    private readonly IRepository<User> _userRepository = userRepository;
    private readonly IAuditLogService _auditLogService = auditLogService;

    public async Task<List<GetUserDto>> GetAllAsync(string search, PaginationParams @params)
    {
        var query = _userRepository.Query()
                                                .AsNoTracking();
        if (!string.IsNullOrEmpty(search))
            query = query.Where(u => EF.Functions.Like(u.Username, $"%{search}%"));

        return await query.ToPagedList(@params)
                          .Select(u => new GetUserDto
                          {
                              Id = u.Id,
                              Username = u.Username,
                              Email = u.Email,
                              CreatedAt = u.CreatedAt,
                              Role = u.Role
                          })
                          .ToListAsync();
    }

    public async Task<GetUserDto> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id)
            ?? throw new CustomException(404, "User not found");
        
        return new GetUserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            Role = user.Role
        };
    }

    public async Task<GetUserForMeDto> GetForMeAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new CustomException(404, "User not found");
        
        return new GetUserForMeDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<GetUserForMeDto> UpdateMeAsync(Guid userId, UpdateUserDto dto)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new CustomException(404, "User not found");
        
        var oldValue = new { user.Email, user.Username };
        user.Email = dto.Email;
        user.Username = dto.Username;
        if (!string.IsNullOrEmpty(dto.Password))
        {
            user.Password = dto.Password.Hash();
        }

        await _userRepository.SaveChangesAsync();
        await _auditLogService.LogUpdateAsync(nameof(User), user.Id, userId, oldValue, user);
        
        return new GetUserForMeDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
        };
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) return false;
        
        var oldValue = new { user.Email, user.Username, user.Role };
        await _userRepository.DeleteAsync(id);
        await _userRepository.SaveChangesAsync();
        await _auditLogService.LogDeleteAsync(nameof(User), user.Id, user.Id, oldValue);
        
        return true;
    }
} 