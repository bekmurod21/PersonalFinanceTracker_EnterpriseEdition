using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceTracker_EnterpriseEdition.Application.Abstractions;
using PersonalFinanceTracker_EnterpriseEdition.Application.Helpers;
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
public class StatisticsController(ITransactionService transactionService, ILogger<StatisticsController> logger) : ControllerBase
{
    private readonly ITransactionService _transactionService = transactionService;
    private readonly ILogger<StatisticsController> _logger = logger;
    private static Guid GetUserId() => HttpContextHelper.UserId;

    [HttpPost("top-categories/excel")]
    public async Task<ActionResult<object>> ExportTopCategoriesToExcel([FromQuery] int year, [FromQuery] int month, [FromQuery] int top = 3)
    {
        try
        {
            var userId = GetUserId();
            var stats = await _transactionService.GetTopCategoryExpensesAsync(userId, year, month, top);
            var fileName = $"top-categories_{userId}_{year}_{month}_{DateTime.UtcNow.Ticks}.xlsx";
            
            _logger.LogInformation("Enqueueing export request for file: {FileName}", fileName);
            
            ExcelExportWorker.Enqueue(new ExcelExportWorker.ExportRequest
            {
                UserId = userId,
                Stats = stats,
                FileName = fileName
            });
            
            return Ok(new { fileName });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ExportTopCategoriesToExcel");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("top-categories/excel/status")]
    public IActionResult GetExportStatus([FromQuery] string fileName)
    {
        try
        {
            var status = ExcelExportWorker.GetStatus(fileName);
            _logger.LogInformation("Export status for {FileName}: {Status}", fileName, status);
            return Ok(new { status });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting export status for {FileName}", fileName);
            return StatusCode(500, "Internal server error");
        }
    }

    [AllowAnonymous]
    [HttpGet("top-categories/excel/download")]
    public IActionResult DownloadExportedFile([FromQuery] string fileName)
    {
        try
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "exports", fileName);
            _logger.LogInformation("Attempting to download file: {FilePath}", filePath);
            
            if (!System.IO.File.Exists(filePath))
            {
                _logger.LogWarning("File not found: {FilePath}", filePath);
                return NotFound();
            }
            
            var bytes = System.IO.File.ReadAllBytes(filePath);
            _logger.LogInformation("Successfully downloaded file: {FileName}, Size: {Size} bytes", fileName, bytes.Length);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading file: {FileName}", fileName);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("trend/excel")]
    public async Task<ActionResult<object>> ExportTrendToExcel([FromQuery] int monthsCount = 6)
    {
        try
        {
            var userId = GetUserId();
            var trends = await _transactionService.GetMonthlyTrendAsync(userId, monthsCount);
            var fileName = $"trend_{userId}_{monthsCount}_{DateTime.UtcNow.Ticks}.xlsx";
            
            _logger.LogInformation("Enqueueing trend export request for file: {FileName}", fileName);
            
            ExcelExportWorker.EnqueueTrend(new ExcelExportWorker.TrendExportRequest
            {
                UserId = userId,
                Trends = trends,
                FileName = fileName
            });
            
            return Ok(new { fileName });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ExportTrendToExcel");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("trend/excel/status")]
    public IActionResult GetTrendExportStatus([FromQuery] string fileName)
    {
        try
        {
            var status = ExcelExportWorker.GetTrendStatus(fileName);
            _logger.LogInformation("Trend export status for {FileName}: {Status}", fileName, status);
            return Ok(new { status });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting trend export status for {FileName}", fileName);
            return StatusCode(500, "Internal server error");
        }
    }

    [AllowAnonymous]
    [HttpGet("trend/excel/download")]
    public IActionResult DownloadTrendExportedFile([FromQuery] string fileName)
    {
        try
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "exports", fileName);
            _logger.LogInformation("Attempting to download trend file: {FilePath}", filePath);
            
            if (!System.IO.File.Exists(filePath))
            {
                _logger.LogWarning("Trend file not found: {FilePath}", filePath);
                return NotFound();
            }
            
            var bytes = System.IO.File.ReadAllBytes(filePath);
            _logger.LogInformation("Successfully downloaded trend file: {FileName}, Size: {Size} bytes", fileName, bytes.Length);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading trend file: {FileName}", fileName);
            return StatusCode(500, "Internal server error");
        }
    }
} 