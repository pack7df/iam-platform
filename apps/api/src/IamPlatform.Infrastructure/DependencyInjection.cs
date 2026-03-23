using IamPlatform.Domain.Identity;
using IamPlatform.Domain.Tenants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IamPlatform.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<InMemoryIdentityStore>();
        services.AddSingleton<InMemoryTenantStore>();
        services.AddSingleton<ISystemUserRepository, InMemorySystemUserRepository>();
        services.AddSingleton<IInvitationRepository, InMemoryInvitationRepository>();
        services.AddSingleton<ITenantRepository, InMemoryTenantRepository>();
        services.AddSingleton<IUserRepository, InMemoryUserRepository>();

        return services;
    }
}
