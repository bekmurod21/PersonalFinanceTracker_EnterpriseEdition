using Newtonsoft.Json;
using PersonalFinanceTracker_EnterpriseEdition.Application.Abstractions;
using PersonalFinanceTracker_EnterpriseEdition.Domain.Entities;

namespace PersonalFinanceTracker_EnterpriseEdition.Application.Services;

public class AuditLogService(IRepository<AuditLog> auditLogRepository) : IAuditLogService
{
    private readonly IRepository<AuditLog> _auditLogRepository = auditLogRepository;

    public async Task LogCreateAsync(string entityName, Guid entityId, Guid userId, object newValue)
    {
        var log = new AuditLog
        {
            UserId = userId,
            Action = "Create",
            EntityName = entityName,
            EntityId = entityId,
            NewValue = newValue != null ? JsonConvert.SerializeObject(newValue) : null
        };
        await _auditLogRepository.AddAsync(log);
        await _auditLogRepository.SaveChangesAsync();
    }

    public async Task LogUpdateAsync(string entityName, Guid entityId, Guid userId, object oldValue, object newValue)
    {
        var log = new AuditLog
        {
            UserId = userId,
            Action = "Update",
            EntityName = entityName,
            EntityId = entityId,
            OldValue = oldValue != null ? JsonConvert.SerializeObject(oldValue) : null,
            NewValue = newValue != null ? JsonConvert.SerializeObject(newValue) : null
        };
        await _auditLogRepository.AddAsync(log);
        await _auditLogRepository.SaveChangesAsync();
    }

    public async Task LogDeleteAsync(string entityName, Guid entityId, Guid userId, object oldValue)
    {
        var log = new AuditLog
        {
            UserId = userId,
            Action = "Delete",
            EntityName = entityName,
            EntityId = entityId,
            OldValue = oldValue != null ? JsonConvert.SerializeObject(oldValue) : null
        };
        await _auditLogRepository.AddAsync(log);
        await _auditLogRepository.SaveChangesAsync();
    }
} 