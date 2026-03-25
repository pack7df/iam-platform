using IamPlatform.Application.Identity;
using IamPlatform.Application.Tenants;
using Microsoft.Extensions.DependencyInjection;

namespace IamPlatform.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ISystemUserBootstrapper, SystemUserBootstrapper>();
        services.AddScoped<ISystemUserInvitation, SystemUserInvitation>();
        services.AddScoped<ITenantAdminRegistration, TenantAdminRegistration>();
        services.AddScoped<ITenantAdminInvitation, TenantAdminInvitation>();

        return services;
    }
}
