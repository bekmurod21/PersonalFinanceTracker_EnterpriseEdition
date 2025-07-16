using Microsoft.AspNetCore.Mvc;
using PersonalFinanceTracker_EnterpriseEdition.Api.Models.Validations;
using PersonalFinanceTracker_EnterpriseEdition.Application.Abstractions;
using PersonalFinanceTracker_EnterpriseEdition.Application.DTOs.Users;

namespace PersonalFinanceTracker_EnterpriseEdition.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("signup")]
    public async Task<ActionResult<SignUpDto>> SignUp([FromBody] SignUpDto dto)
    {
        var (IsValid, Message) = dto.Password.IsStrong();
        if (!IsValid)
            return BadRequest(Message);
        var user = await _authService.SignUpAsync(dto);
        return Ok(new { user.Id, user.Email, user.Username });
    }

    [HttpPost("signin")]
    public async Task<ActionResult<object>> SignIn([FromBody] SignInDto dto)
    {
        var (IsValid, Message) = dto.Password.IsStrong();
        if (!IsValid)
            return BadRequest(Message);
        var (AccessToken, RefreshToken) = await _authService.SignInAsync(dto);
        return Ok(new { AccessToken, RefreshToken });
    }
}