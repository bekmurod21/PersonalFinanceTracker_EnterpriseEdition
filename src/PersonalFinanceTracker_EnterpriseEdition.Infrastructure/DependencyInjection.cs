using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using PersonalFinanceTracker_EnterpriseEdition.Application.Abstractions;
using PersonalFinanceTracker_EnterpriseEdition.Infrastructure.Persistense;
using PersonalFinanceTracker_EnterpriseEdition.Infrastructure.Services;
using PersonalFinanceTracker_EnterpriseEdition.Domain.Helpers;

namespace PersonalFinanceTracker_EnterpriseEdition.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(EnvironmentHelper.DatabaseUrl);
            options.ConfigureWarnings(warnings =>
                                warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        });
        
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped(typeof(IRepository<>),typeof(Repository<>));

        return services;
    }
}