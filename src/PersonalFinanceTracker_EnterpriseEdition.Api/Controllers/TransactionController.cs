using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceTracker_EnterpriseEdition.Application.Abstractions;
using PersonalFinanceTracker_EnterpriseEdition.Application.DTOs.Transactions;
using PersonalFinanceTracker_EnterpriseEdition.Application.Helpers;
using PersonalFinanceTracker_EnterpriseEdition.Domain.Configurations;
using System.Security.Claims;

namespace PersonalFinanceTracker_EnterpriseEdition.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "UserOnly")]
public class TransactionController(ITransactionService transactionService) : ControllerBase
{
    private readonly ITransactionService _transactionService = transactionService;
    private static Guid GetUserId() => HttpContextHelper.UserId;

    [HttpGet]
    public async Task<ActionResult<List<GetTransactionDto>>> GetAll([FromQuery] PaginationParams @params, [FromQuery] string? sort, [FromQuery] string? filter)
    {
        var items = await _transactionService.GetAllAsync( GetUserId(),@params, sort, filter);
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetTransactionDto>> GetById(Guid id)
    {
        var result = await _transactionService.GetByIdAsync(GetUserId(), id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<GetTransactionDto>> Create([FromBody] CreateTransactionDto dto)
    {
        var result = await _transactionService.CreateAsync(GetUserId(),dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] UpdateTransactionDto dto)
    {
        var result = await _transactionService.UpdateAsync(GetUserId(), id, dto);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = await _transactionService.DeleteAsync(GetUserId(), id);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetMonthlySummary([FromQuery] int year, [FromQuery] int month)
    {
        var summary = await _transactionService.GetMonthlySummaryAsync(GetUserId(), year, month);
        return Ok(summary);
    }

    [HttpGet("top-categories")]
    public async Task<IActionResult> GetTopCategoryExpenses([FromQuery] int year, [FromQuery] int month, [FromQuery] int top = 3)
    {
        var stats = await _transactionService.GetTopCategoryExpensesAsync(GetUserId(), year, month, top);
        return Ok(stats);
    }

    [HttpGet("trend")]
    public async Task<IActionResult> GetMonthlyTrend([FromQuery] int monthsCount = 6)
    {
        var trend = await _transactionService.GetMonthlyTrendAsync(GetUserId(), monthsCount);
        return Ok(trend);
    }
} 