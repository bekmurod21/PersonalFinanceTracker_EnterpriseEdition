using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
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
    private readonly string _exportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "PersonalFinanceTracker_EnterpriseEdition.Api", "wwwroot", "exports");

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
        Directory.CreateDirectory(_exportPath);
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_queue.TryDequeue(out var request))
            {
                try
                {
                    var bytes = ExcelExportHelper.ExportTopCategoryExpensesToExcel(request.Stats);
                    var filePath = Path.Combine(_exportPath, request.FileName);
                    await File.WriteAllBytesAsync(filePath, bytes, stoppingToken);
                    _fileStatus[request.FileName] = "ready";
                }
                catch
                {
                    _fileStatus[request.FileName] = "error";
                }
            }
            else if (_trendQueue.TryDequeue(out var trendRequest))
            {
                try
                {
                    var bytes = ExcelExportHelper.ExportMonthlyTrendToExcel(trendRequest.Trends);
                    var filePath = Path.Combine(_exportPath, trendRequest.FileName);
                    await File.WriteAllBytesAsync(filePath, bytes, stoppingToken);
                    _trendFileStatus[trendRequest.FileName] = "ready";
                }
                catch
                {
                    _trendFileStatus[trendRequest.FileName] = "error";
                }
            }
            else
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
} 