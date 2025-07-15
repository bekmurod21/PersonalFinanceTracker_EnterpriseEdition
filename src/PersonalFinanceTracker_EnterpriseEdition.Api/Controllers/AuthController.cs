using Microsoft.AspNetCore.Mvc;
using PersonalFinanceTracker_EnterpriseEdition.Application.Abstractions;
using PersonalFinanceTracker_EnterpriseEdition.Application.DTOs.Users;

namespace PersonalFinanceTracker_EnterpriseEdition.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("signup")]
    public async Task<IActionResult> SignUp([FromBody] SignUpDto dto)
    {
        var user = await _authService.SignUpAsync(dto);
        return Ok(new { user.Id, user.Email, user.Username });
    }

    [HttpPost("signin")]
    public async Task<IActionResult> SignIn([FromBody] SignInDto dto)
    {
        var tokens = await _authService.SignInAsync(dto);
        return Ok(new { tokens.AccessToken, tokens.RefreshToken });
    }
}