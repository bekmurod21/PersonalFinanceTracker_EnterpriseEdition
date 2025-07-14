using PersonalFinanceTracker_EnterpriseEdition.Api.Models;
using PersonalFinanceTracker_EnterpriseEdition.Domain.Exceptions;

namespace PersonalFinanceTracker_EnterpriseEdition.Api.Middlewares;

public class ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
{
    private readonly RequestDelegate next = next;
    private readonly ILogger<ExceptionHandlerMiddleware> logger = logger;

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (CustomException exception)
        {
            this.logger.LogError("{CustomException}\n\n\t", exception);
            context.Response.StatusCode = exception.StatusCode;
            await context.Response.WriteAsJsonAsync(new Response
            {
                StatusCode = exception.StatusCode,
                Message = exception.Message
            });
        }
        catch (Exception exception)
        {
            this.logger.LogError("{Exception}\n\n\t", exception);
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new Response
            {
                StatusCode = 500,
                Message = exception.Message
            });
        }
    }
}