using IamPlatform.Application.Identity;
using IamPlatform.Application.Tenants;
using Microsoft.Extensions.DependencyInjection;

namespace IamPlatform.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<SystemUserBootstrapper>();
        services.AddScoped<SystemUserInvitation>();
        services.AddScoped<TenantAdminRegistration>();
        services.AddScoped<TenantAdminInvitation>();

        return services;
    }
}
