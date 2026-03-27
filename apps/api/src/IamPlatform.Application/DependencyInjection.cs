using IamPlatform.Application.Authorization.Resources;
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

        // Resource Management
        services.AddScoped<ICreateResourceHandler, CreateResourceHandler>();
        services.AddScoped<IUpdateResourceHandler, UpdateResourceHandler>();
        services.AddScoped<IDeleteResourceHandler, DeleteResourceHandler>();
        services.AddScoped<IGetResourceHandler, GetResourceHandler>();
        services.AddScoped<IListResourcesHandler, ListResourcesHandler>();

        return services;
    }
}
