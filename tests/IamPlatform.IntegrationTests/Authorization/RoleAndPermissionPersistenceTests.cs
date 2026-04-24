using IamPlatform.Domain.Applications;
using IamPlatform.Domain.Authorization;
using IamPlatform.Domain.Operations;
using IamPlatform.Domain.Tenants;
using IamPlatform.Domain.Users;
using IamPlatform.Domain.Common;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Action = IamPlatform.Domain.Applications.Action;

namespace IamPlatform.IntegrationTests.Authorization;

public class RoleAndPermissionPersistenceTests : BaseIntegrationTest
{
    [Fact]
    public async Task Should_PersistAndRetrieve_Roles_And_Permissions()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var tenantRepo = scope.ServiceProvider.GetRequiredService<ITenantRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var tenant = new Tenant("Auth Tenant", "auth-tenant");
        await tenantRepo.AddAsync(tenant);
        await unitOfWork.SaveChangesAsync();

        var userRepo = tenantRepo.GetUserRepository(tenant.Id);
        var roleRepo = tenantRepo.GetRoleRepository(tenant.Id);
        var appRepo = tenantRepo.GetApplicationRepository(tenant.Id);
        var opRepo = tenantRepo.GetOperationRepository(tenant.Id);

        // 1. Create User and Role
        var user = new User(tenant.Id, "perm-user", "perm@test.com", "hash");
        await userRepo.AddAsync(user);

        var adminRole = new Role(tenant.Id, "Administrator");
        await roleRepo.AddAsync(adminRole);

        // 2. Create Infrastructure (Op -> App -> Res -> Action)
        var readOp = new Operation(tenant.Id, "Read", "read");
        await opRepo.AddAsync(readOp);

        var app = new IamPlatform.Domain.Applications.Application(tenant.Id, "Auth App", "auth-app");
        await appRepo.AddAsync(app);
        await unitOfWork.SaveChangesAsync();

        var resRepo = appRepo.GetResourceRepository(app.Id);
        var resource = new Resource(app.Id, "Secret Doc", "secret-doc");
        await resRepo.AddAsync(resource);
        await unitOfWork.SaveChangesAsync();

        var actionRepo = resRepo.GetActionRepository(resource.Id);
        var readAction = new Action(resource.Id, readOp.Id);
        await actionRepo.AddAsync(readAction);
        await unitOfWork.SaveChangesAsync();

        var permRepo = actionRepo.GetPermissionRepository(readAction.Id);

        // Act - Assign Permissions
        var userPermission = new Permission(readAction.Id, PermissionOutcome.Allowed, userId: user.Id);
        var rolePermission = new Permission(readAction.Id, PermissionOutcome.Denied, roleId: adminRole.Id);

        await permRepo.AddAsync(userPermission);
        await permRepo.AddAsync(rolePermission);
        await unitOfWork.SaveChangesAsync();

        // Assert
        var retrievedRole = await roleRepo.GetByNameAsync("Administrator");
        retrievedRole.Should().NotBeNull();

        var allPermissions = (await permRepo.GetAllAsync()).ToList();
        allPermissions.Should().HaveCount(2);

        var userPerm = allPermissions.FirstOrDefault(p => p.UserId == user.Id);
        userPerm.Should().NotBeNull();
        userPerm!.Outcome.Should().Be(PermissionOutcome.Allowed);

        var rolePerm = allPermissions.FirstOrDefault(p => p.RoleId == adminRole.Id);
        rolePerm.Should().NotBeNull();
        rolePerm!.Outcome.Should().Be(PermissionOutcome.Denied);
    }
}
