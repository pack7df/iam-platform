using IamPlatform.Domain.Authorization;
using IamPlatform.Domain.Identity;
using IamPlatform.Domain.Tenants;
using IamPlatform.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IamPlatform.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure DbContext with PostgreSQL
        var connectionString = configuration.GetConnectionString("IamPlatform");
        services.AddDbContext<IamPlatformDbContext>(options =>
            options.UseNpgsql(connectionString));
            
         // In-memory stores (will be replaced by EF repositories in later tasks)
         services.AddSingleton<InMemoryTenantStore>();
         services.AddSingleton<InMemoryIdentityStore>();
         services.AddSingleton<IInvitationRepository, InMemoryInvitationRepository>();
         services.AddSingleton<ITenantRepository, InMemoryTenantRepository>();
         services.AddSingleton<IUserRepository, InMemoryUserRepository>();

        return services;
    }
}
