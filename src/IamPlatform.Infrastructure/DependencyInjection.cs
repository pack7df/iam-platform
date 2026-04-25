using IamPlatform.Domain.Common;
using IamPlatform.Domain.Tenants;
using IamPlatform.Domain.Users;
using IamPlatform.Infrastructure.Persistence;
using IamPlatform.Infrastructure.Persistence.Interceptors;
using IamPlatform.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IamPlatform.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddScoped<AuditInterceptor>();

        services.AddDbContext<IamPlatformDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString)
                   .AddInterceptors(sp.GetRequiredService<AuditInterceptor>());
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ITenantRepository, TenantRepository>();


        return services;

    }
}
