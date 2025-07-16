using PersonalFinanceTracker_EnterpriseEdition.Api.Models;
using PersonalFinanceTracker_EnterpriseEdition.Domain.Exceptions;
using PersonalFinanceTracker_EnterpriseEdition.Api.Helpers;
using System.Diagnostics;

namespace PersonalFinanceTracker_EnterpriseEdition.Api.Middlewares;

public class ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
{
    private readonly RequestDelegate next = next;
    private readonly ILogger<ExceptionHandlerMiddleware> logger = logger;

    public async Task Invoke(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var originalPath = context.Request.Path;
        var method = context.Request.Method;
        var userId = context.User?.FindFirst("Id")?.Value ?? "anonymous";
        var ipAddress = GetIpAddress(context);

        try
        {
            await next(context);
            
            stopwatch.Stop();
            logger.LogApiRequest(method, originalPath, userId, context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
        }
        catch (CustomException exception)
        {
            stopwatch.Stop();
            logger.LogApiError(method, originalPath, exception, userId);
            logger.LogSecurityEvent("CustomException", userId, ipAddress, exception.Message);
            
            context.Response.StatusCode = exception.StatusCode;
            await context.Response.WriteAsJsonAsync(new Response
            {
                StatusCode = exception.StatusCode,
                Message = exception.Message
            });
        }
        catch (Exception exception)
        {
            stopwatch.Stop();
            logger.LogApiError(method, originalPath, exception, userId);
            logger.LogSecurityEvent("UnhandledException", userId, ipAddress, exception.Message);
            
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new Response
            {
                StatusCode = 500,
                Message = "Internal server error occurred."
            });
        }
    }

    private static string GetIpAddress(HttpContext context)
    {
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',')[0].Trim();
        }

        var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIp))
        {
            return realIp;
        }

        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}