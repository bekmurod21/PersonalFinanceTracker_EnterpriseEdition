using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS;
using PersonalFinanceTracker_EnterpriseEdition.Application.Abstractions;
using PersonalFinanceTracker_EnterpriseEdition.Application.DTOs.Categories;
using PersonalFinanceTracker_EnterpriseEdition.Application.Helpers;
using PersonalFinanceTracker_EnterpriseEdition.Domain.Configurations;
using System.Security.Claims;

namespace PersonalFinanceTracker_EnterpriseEdition.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "UserOnly")]
public class CategoryController(ICategoryService categoryService) : ControllerBase
{
    private readonly ICategoryService _categoryService = categoryService;

    private static Guid GetUserId() => HttpContextHelper.UserId;

    [HttpGet]
    public async Task<ActionResult<List<GetCategoryDto>>> GetAll([FromQuery] PaginationParams @params,[FromQuery]string? search)
    {
        var result = await _categoryService.GetAllAsync(GetUserId(),search,@params);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetCategoryDto>> GetById(Guid id)
    {
        var result = await _categoryService.GetByIdAsync(id, GetUserId());
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<GetCategoryDto>> Create([FromBody] CreateCategoryDto dto)
    {
        var result = await _categoryService.CreateAsync(dto, GetUserId());
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<GetCategoryDto>> Update(Guid id, [FromBody] UpdateCategoryDto dto)
    {
        var result = await _categoryService.UpdateAsync(id, dto, GetUserId());
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var success = await _categoryService.DeleteAsync(id, GetUserId());
        if (!success) return NotFound();
        return NoContent();
    }
} 