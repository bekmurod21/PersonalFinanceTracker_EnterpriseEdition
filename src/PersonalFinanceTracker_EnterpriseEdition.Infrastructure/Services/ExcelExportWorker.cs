using System.Collections.Concurrent;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PersonalFinanceTracker_EnterpriseEdition.Application.Helpers;
using PersonalFinanceTracker_EnterpriseEdition.Application.DTOs.Transactions;

namespace PersonalFinanceTracker_EnterpriseEdition.Infrastructure.Services;

public class ExcelExportWorker : BackgroundService
{
    public class ExportRequest
    {
        public Guid UserId { get; set; }
        public List<CategoryExpenseStatDto> Stats { get; set; } = new();
        public string FileName { get; set; } = string.Empty;
    }

    public class TrendExportRequest
    {
        public Guid UserId { get; set; }
        public List<MonthlyTrendDto> Trends { get; set; } = new();
        public string FileName { get; set; } = string.Empty;
    }

    private static readonly ConcurrentQueue<ExportRequest> _queue = new();
    private static readonly ConcurrentDictionary<string, string> _fileStatus = new();
    private static readonly ConcurrentQueue<TrendExportRequest> _trendQueue = new();
    private static readonly ConcurrentDictionary<string, string> _trendFileStatus = new();
    private readonly ILogger<ExcelExportWorker> _logger;
    private readonly string _exportPath;

    public ExcelExportWorker(ILogger<ExcelExportWorker> logger)
    {
        _logger = logger;
        
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var projectRoot = Directory.GetCurrentDirectory();
        
        var wwwrootPath = Path.Combine(projectRoot, "wwwroot", "exports");
        if (!Directory.Exists(wwwrootPath))
        {
            wwwrootPath = Path.Combine(baseDirectory, "wwwroot", "exports");
        }
        
        _exportPath = wwwrootPath;
        _logger.LogInformation("ExcelExportWorker initialized with export path: {ExportPath}", _exportPath);
    }

    public static void Enqueue(ExportRequest request)
    {
        _queue.Enqueue(request);
        _fileStatus[request.FileName] = "processing";
    }

    public static string GetStatus(string fileName)
    {
        return _fileStatus.TryGetValue(fileName, out var status) ? status : "not_found";
    }

    public static void EnqueueTrend(TrendExportRequest request)
    {
        _trendQueue.Enqueue(request);
        _trendFileStatus[request.FileName] = "processing";
    }

    public static string GetTrendStatus(string fileName)
    {
        return _trendFileStatus.TryGetValue(fileName, out var status) ? status : "not_found";
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            if (!Directory.Exists(_exportPath))
            {
                Directory.CreateDirectory(_exportPath);
                _logger.LogInformation("Created export directory: {ExportPath}", _exportPath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create export directory: {ExportPath}", _exportPath);
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            if (_queue.TryDequeue(out var request))
            {
                await ProcessExportRequest(request, stoppingToken);
            }
            else if (_trendQueue.TryDequeue(out var trendRequest))
            {
                await ProcessTrendExportRequest(trendRequest, stoppingToken);
            }
            else
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }

    private async Task ProcessExportRequest(ExportRequest request, CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Processing export request for file: {FileName}", request.FileName);
            
            var bytes = ExcelExportHelper.ExportTopCategoryExpensesToExcel(request.Stats);
            var filePath = Path.Combine(_exportPath, request.FileName);
            
            await File.WriteAllBytesAsync(filePath, bytes, stoppingToken);
            _fileStatus[request.FileName] = "ready";
            
            _logger.LogInformation("Successfully created export file: {FilePath}", filePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process export request for file: {FileName}", request.FileName);
            _fileStatus[request.FileName] = "error";
        }
    }

    private async Task ProcessTrendExportRequest(TrendExportRequest request, CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Processing trend export request for file: {FileName}", request.FileName);
            
            var bytes = ExcelExportHelper.ExportMonthlyTrendToExcel(request.Trends);
            var filePath = Path.Combine(_exportPath, request.FileName);
            
            await File.WriteAllBytesAsync(filePath, bytes, stoppingToken);
            _trendFileStatus[request.FileName] = "ready";
            
            _logger.LogInformation("Successfully created trend export file: {FilePath}", filePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process trend export request for file: {FileName}", request.FileName);
            _trendFileStatus[request.FileName] = "error";
        }
    }
} 