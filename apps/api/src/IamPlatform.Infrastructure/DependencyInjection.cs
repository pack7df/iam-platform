using IamPlatform.Application.Common.Interfaces;
using IamPlatform.Domain.Authorization;
using IamPlatform.Domain.Common;
using IamPlatform.Domain.Identity;
using IamPlatform.Domain.Tenants;
using IamPlatform.Infrastructure.Communication;
using IamPlatform.Infrastructure.Persistence;
using IamPlatform.Infrastructure.Persistence.Repositories;
using IamPlatform.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IamPlatform.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Infrastructure Services
        services.AddHttpContextAccessor();
        services.AddScoped<IEmailSender, FakeEmailSender>();
        services.AddScoped<ICurrentUserContext, CurrentUserContext>();

        // Configure DbContext with PostgreSQL
        var connectionString = configuration.GetConnectionString("IamPlatform");
        services.AddDbContext<IamPlatformDbContext>(options =>
            options.UseNpgsql(connectionString));
            
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<IamPlatformDbContext>());
            
         // Repositories (EF Core)
         services.AddScoped<IResourceRepository, ResourceRepository>();
         services.AddScoped<IInvitationRepository, InvitationRepository>();
         services.AddScoped<ITenantRepository, TenantRepository>();
         services.AddScoped<IUserRepository, UserRepository>();
         services.AddScoped<IAuthorizationRuleRepository, AuthorizationRuleRepository>();
         services.AddScoped<IUserRoleAssignmentRepository, UserRoleAssignmentRepository>();
         services.AddScoped<IRoleRepository, RoleRepository>();
         services.AddScoped<IApplicationRepository, ApplicationRepository>();
         services.AddScoped<IOperationRepository, OperationRepository>();

         // Domain Services
         services.AddScoped<IAuthorizationEngine, AuthorizationEngine>();
         services.AddScoped<IAdminPrivilegeService, AdminPrivilegeService>();

        return services;
    }
}
