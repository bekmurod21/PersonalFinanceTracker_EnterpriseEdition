using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceTracker_EnterpriseEdition.Application.Abstractions;
using PersonalFinanceTracker_EnterpriseEdition.Application.DTOs.Users;
using PersonalFinanceTracker_EnterpriseEdition.Domain.Configurations;
using PersonalFinanceTracker_EnterpriseEdition.Application.Helpers;
using System.Security.Claims;

namespace PersonalFinanceTracker_EnterpriseEdition.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [HttpGet, Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<GetUserDto>>> GetAll([FromQuery] string? search, [FromQuery] PaginationParams @params)
        {
            var users = await _userService.GetAllAsync(search, @params);
            return Ok(users);
        }

        [HttpGet("{id}"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<GetUserDto>> GetById(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);
            return Ok(user);
        }

        [HttpDelete("{id}"), Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var result = await _userService.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpGet("me"),Authorize]
        public async Task<ActionResult<GetUserForMeDto>> GetMe()
        {
            var userId = GetUserId();
            var user = await _userService.GetForMeAsync(userId);
            return Ok(user);
        }

        [HttpPut("me"),Authorize]
        public async Task<ActionResult<GetUserForMeDto>> UpdateMe([FromBody] UpdateUserDto dto)
        {
            var userId = GetUserId();
            var user = await _userService.UpdateMeAsync(userId, dto);
            return Ok(user);
        }

        private static Guid GetUserId() => HttpContextHelper.UserId;
    }
}
