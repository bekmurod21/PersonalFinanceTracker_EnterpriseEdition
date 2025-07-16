namespace PersonalFinanceTracker_EnterpriseEdition.Application.Abstractions;

public interface IAuditLogService
{
    Task LogCreateAsync(string entityName, Guid entityId, Guid userId, object newValue);
    Task LogUpdateAsync(string entityName, Guid entityId, Guid userId, object oldValue, object newValue);
    Task LogDeleteAsync(string entityName, Guid entityId, Guid userId, object oldValue);
} 