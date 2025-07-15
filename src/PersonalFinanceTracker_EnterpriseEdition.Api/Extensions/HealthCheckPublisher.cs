using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace PersonalFinanceTracker_EnterpriseEdition.Api.Extensions;

public class HealthCheckPublisher(ILogger<HealthCheckPublisher> logger) : IHealthCheckPublisher
{
    public async Task PublishAsync(HealthReport report, CancellationToken cancellationToken)
    {
        foreach (var entry in report.Entries)
        {
            if (entry.Value.Status == HealthStatus.Unhealthy)
            {
                string message = $"Health check failed for {entry.Key}: {entry.Value.Description}";
                logger.LogError(message);
            }
        }
    }
}
