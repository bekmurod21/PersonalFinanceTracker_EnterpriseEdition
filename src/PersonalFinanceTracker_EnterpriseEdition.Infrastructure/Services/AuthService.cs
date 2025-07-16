using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PersonalFinanceTracker_EnterpriseEdition.Application.Abstractions;
using PersonalFinanceTracker_EnterpriseEdition.Application.DTOs.Users;
using PersonalFinanceTracker_EnterpriseEdition.Domain.Entities;
using PersonalFinanceTracker_EnterpriseEdition.Application.Helpers;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker_EnterpriseEdition.Domain.Exceptions;

namespace PersonalFinanceTracker_EnterpriseEdition.Infrastructure.Services;

public class AuthService(IConfiguration configuration, IRepository<User> userRepository) : IAuthService
{
    private readonly IConfiguration _configuration = configuration;
    private readonly IRepository<User> _userRepository = userRepository;

    public string GenerateJwtToken(User user)
    {
        var tokenHendler = new JwtSecurityTokenHandler();
        var tokenKey = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]);
        var tokenDisctiptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim("Id", user.Id.ToString()),
                new Claim(ClaimTypes.Role, user?.Role.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
            }),
            Audience = _configuration["JWT:Audience"],
            Issuer = _configuration["JWT:Issuer"],
            IssuedAt = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddDays(int.Parse(_configuration["JWT:Expire"])),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHendler.CreateToken(tokenDisctiptor);
        return tokenHendler.WriteToken(token);
    }

    public async Task<User> SignUpAsync(SignUpDto dto)
    {
        var exists = await _userRepository.Query(u => u.Email == dto.Email || u.Username == dto.Username).AnyAsync();
        if (exists) throw new CustomException(409, "User already exists");
        var user = new User
        {
            Email = dto.Email,
            Username = dto.Username,
            Password = dto.Password.Hash(),
            Role = Domain.Enums.ERole.User
        };
        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();
        return user;
    }

    public async Task<(string AccessToken, string RefreshToken)> SignInAsync(SignInDto dto)
    {
        var user = await _userRepository.Query(u => u.Email == dto.EmailOrUserName || u.Username == dto.EmailOrUserName)
            .FirstOrDefaultAsync()
            ?? throw new CustomException(404, "User not found");
        if (!PasswordHelper.Verify(dto.Password,user.Password)) throw new CustomException(401, "Password is incorrect");
        var accessToken = GenerateJwtToken(user);
        var refreshToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        return (accessToken, refreshToken);
    }
}