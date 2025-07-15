using Microsoft.Extensions.DependencyInjection;
using PersonalFinanceTracker_EnterpriseEdition.Application.Abstractions;
using PersonalFinanceTracker_EnterpriseEdition.Application.Services;
using Microsoft.Extensions.Caching.Distributed;

namespace PersonalFinanceTracker_EnterpriseEdition.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {

        services.AddScoped<IAuditLogService, AuditLogService>();
        services.AddScoped<ITransactionService, TransactionService>(provider =>
        {
            var transactionRepo = provider.GetRequiredService<IRepository<Domain.Entities.Transaction>>();
            var categoryRepo = provider.GetRequiredService<IRepository<Domain.Entities.Category>>();
            var auditLogService = provider.GetRequiredService<IAuditLogService>();
            var cache = provider.GetRequiredService<IDistributedCache>();
            return new TransactionService(transactionRepo, categoryRepo, auditLogService, cache);
        });
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}