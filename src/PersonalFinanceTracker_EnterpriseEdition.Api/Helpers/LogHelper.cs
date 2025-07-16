using System.Diagnostics;

namespace PersonalFinanceTracker_EnterpriseEdition.Api.Helpers;

public static class LogHelper
{
    public static void LogApiRequest<T>(this ILogger<T> logger, string method, string path, string userId = "", int statusCode = 0, long durationMs = 0)
    {
        logger.LogInformation(
            "API Request: {Method} {Path} | User: {UserId} | Status: {StatusCode} | Duration: {DurationMs}ms",
            method, path, userId, statusCode, durationMs);
    }

    public static void LogApiError<T>(this ILogger<T> logger, string method, string path, Exception ex, string userId = "")
    {
        logger.LogError(ex,
            "API Error: {Method} {Path} | User: {UserId} | Error: {ErrorMessage}",
            method, path, userId, ex.Message);
    }

    public static void LogDatabaseOperation<T>(this ILogger<T> logger, string operation, string entity, string userId = "", long durationMs = 0)
    {
        logger.LogInformation(
            "Database Operation: {Operation} on {Entity} | User: {UserId} | Duration: {DurationMs}ms",
            operation, entity, userId, durationMs);
    }

    public static void LogCacheOperation<T>(this ILogger<T> logger, string operation, string key, bool success = true)
    {
        var level = success ? LogLevel.Information : LogLevel.Warning;
        logger.Log(level,
            "Cache Operation: {Operation} | Key: {Key} | Success: {Success}",
            operation, key, success);
    }

    public static void LogAuditEvent<T>(this ILogger<T> logger, string action, string entity, string entityId, string userId, string details = "")
    {
        logger.LogInformation(
            "Audit Event: {Action} | Entity: {Entity} | EntityId: {EntityId} | User: {UserId} | Details: {Details}",
            action, entity, entityId, userId, details);
    }

    public static void LogPerformanceMetric<T>(this ILogger<T> logger, string operation, long durationMs, string userId = "")
    {
        var level = durationMs > 1000 ? LogLevel.Warning : LogLevel.Information;
        logger.Log(level,
            "Performance: {Operation} | Duration: {DurationMs}ms | User: {UserId}",
            operation, durationMs, userId);
    }

    public static void LogSecurityEvent<T>(this ILogger<T> logger, string eventType, string userId, string ipAddress, string details = "")
    {
        logger.LogWarning(
            "Security Event: {EventType} | User: {UserId} | IP: {IpAddress} | Details: {Details}",
            eventType, userId, ipAddress, details);
    }

    public static void LogBusinessEvent<T>(this ILogger<T> logger, string eventType, string userId, object data)
    {
        logger.LogInformation(
            "Business Event: {EventType} | User: {UserId} | Data: {@Data}",
            eventType, userId, data);
    }

    public static IDisposable LogOperation<T>(this ILogger<T> logger, string operationName, string userId = "")
    {
        var stopwatch = Stopwatch.StartNew();
        logger.LogInformation("Starting operation: {Operation} | User: {UserId}", operationName, userId);
        
        return new DisposableOperation(logger, operationName, userId, stopwatch);
    }

    private class DisposableOperation(ILogger logger, string operationName, string userId, Stopwatch stopwatch) : IDisposable
    {
        private readonly ILogger _logger = logger;
        private readonly string _operationName = operationName;
        private readonly string _userId = userId;
        private readonly Stopwatch _stopwatch = stopwatch;

        public void Dispose()
        {
            _stopwatch.Stop();
            _logger.LogInformation(
                "Completed operation: {Operation} | User: {UserId} | Duration: {DurationMs}ms",
                _operationName, _userId, _stopwatch.ElapsedMilliseconds);
        }
    }
} 