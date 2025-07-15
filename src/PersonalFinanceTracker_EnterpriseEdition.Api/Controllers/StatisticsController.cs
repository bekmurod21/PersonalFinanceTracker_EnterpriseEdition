using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceTracker_EnterpriseEdition.Application.Abstractions;
using PersonalFinanceTracker_EnterpriseEdition.Application.Services;
using PersonalFinanceTracker_EnterpriseEdition.Infrastructure.Services;
using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PersonalFinanceTracker_EnterpriseEdition.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StatisticsController(ITransactionService transactionService) : ControllerBase
{
    private readonly ITransactionService _transactionService = transactionService;

    [HttpPost("top-categories/excel")]
    public async Task<IActionResult> ExportTopCategoriesToExcel([FromQuery] int year, [FromQuery] int month, [FromQuery] int top = 3)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var stats = await _transactionService.GetTopCategoryExpensesAsync(userId, year, month, top);
        var fileName = $"top-categories_{userId}_{year}_{month}_{DateTime.UtcNow.Ticks}.xlsx";
        ExcelExportWorker.Enqueue(new ExcelExportWorker.ExportRequest
        {
            UserId = userId,
            Stats = stats,
            FileName = fileName
        });
        return Ok(new { fileName });
    }

    [HttpGet("top-categories/excel/status")]
    public IActionResult GetExportStatus([FromQuery] string fileName)
    {
        var status = ExcelExportWorker.GetStatus(fileName);
        return Ok(new { status });
    }

    [AllowAnonymous]
    [HttpGet("top-categories/excel/download")]
    public IActionResult DownloadExportedFile([FromQuery] string fileName)
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "src", "PersonalFinanceTracker_EnterpriseEdition.Api", "wwwroot", "exports", fileName);
        if (!System.IO.File.Exists(filePath))
            return NotFound();
        var bytes = System.IO.File.ReadAllBytes(filePath);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    [HttpPost("trend/excel")]
    public async Task<IActionResult> ExportTrendToExcel([FromQuery] int monthsCount = 6)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var trends = await _transactionService.GetMonthlyTrendAsync(userId, monthsCount);
        var fileName = $"trend_{userId}_{monthsCount}_{DateTime.UtcNow.Ticks}.xlsx";
        ExcelExportWorker.EnqueueTrend(new ExcelExportWorker.TrendExportRequest
        {
            UserId = userId,
            Trends = trends,
            FileName = fileName
        });
        return Ok(new { fileName });
    }

    [HttpGet("trend/excel/status")]
    public IActionResult GetTrendExportStatus([FromQuery] string fileName)
    {
        var status = ExcelExportWorker.GetTrendStatus(fileName);
        return Ok(new { status });
    }

    [AllowAnonymous]
    [HttpGet("trend/excel/download")]
    public IActionResult DownloadTrendExportedFile([FromQuery] string fileName)
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "src", "PersonalFinanceTracker_EnterpriseEdition.Api", "wwwroot", "exports", fileName);
        if (!System.IO.File.Exists(filePath))
            return NotFound();
        var bytes = System.IO.File.ReadAllBytes(filePath);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }
} 