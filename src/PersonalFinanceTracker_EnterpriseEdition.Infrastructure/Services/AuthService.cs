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

namespace PersonalFinanceTracker_EnterpriseEdition.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly IRepository<User> _userRepository;

    public AuthService(IConfiguration configuration, IRepository<User> userRepository)
    {
        _configuration = configuration;
        _userRepository = userRepository;
    }

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
                new Claim(ClaimTypes.Name, user.FirstName),
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
        // Email yoki UserName unikal bo‘lishi kerak
        var exists = await _userRepository.Query(u => u.Email == dto.Email || u.UserName == dto.UserName).AnyAsync();
        if (exists) throw new Exception("User already exists");
        var user = new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            UserName = dto.UserName,
            Password = dto.Password.Hash(),
            Role = Domain.Enums.ERole.User
        };
        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();
        return user;
    }

    public async Task<(string AccessToken, string RefreshToken)> SignInAsync(SignInDto dto)
    {
        var user = await _userRepository.Query(u => u.Email == dto.EmailOrUserName || u.UserName == dto.EmailOrUserName).FirstOrDefaultAsync();
        if (user == null) throw new Exception("User not found");
        if (user.Password != dto.Password.Hash()) throw new Exception("Password is incorrect");
        var accessToken = GenerateJwtToken(user);
        // Refresh token generatsiyasi (oddiy random string)
        var refreshToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        // (Refresh tokenni DB ga saqlash logikasi keyinroq qo‘shiladi)
        return (accessToken, refreshToken);
    }
}