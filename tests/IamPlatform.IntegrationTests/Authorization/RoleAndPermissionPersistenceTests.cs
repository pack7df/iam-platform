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

        await permRepo.SetAsync(userPermission);
        await permRepo.SetAsync(rolePermission);
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

    [Fact]
    public async Task Should_UpdateOutcome_When_PermissionAlreadyExists()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var tenantRepo = scope.ServiceProvider.GetRequiredService<ITenantRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var tenant = new Tenant("Upsert Tenant", "upsert-tenant");
        await tenantRepo.AddAsync(tenant);
        await unitOfWork.SaveChangesAsync();

        var userRepo = tenantRepo.GetUserRepository(tenant.Id);
        var user = new User(tenant.Id, "upsert-user", "upsert@test.com", "hash");
        await userRepo.AddAsync(user);

        var opRepo = tenantRepo.GetOperationRepository(tenant.Id);
        var writeOp = new Operation(tenant.Id, "Write", "write");
        await opRepo.AddAsync(writeOp);

        var appRepo = tenantRepo.GetApplicationRepository(tenant.Id);
        var app = new IamPlatform.Domain.Applications.Application(tenant.Id, "Upsert App", "upsert-app");
        await appRepo.AddAsync(app);
        await unitOfWork.SaveChangesAsync();

        var resRepo = appRepo.GetResourceRepository(app.Id);
        var resource = new Resource(app.Id, "File", "file");
        await resRepo.AddAsync(resource);
        await unitOfWork.SaveChangesAsync();

        var actionRepo = resRepo.GetActionRepository(resource.Id);
        var writeAction = new Action(resource.Id, writeOp.Id);
        await actionRepo.AddAsync(writeAction);
        await unitOfWork.SaveChangesAsync();

        var permRepo = actionRepo.GetPermissionRepository(writeAction.Id);

        // 1. Initial Set (Create)
        var perm = new Permission(writeAction.Id, PermissionOutcome.Denied, userId: user.Id);
        await permRepo.SetAsync(perm);
        await unitOfWork.SaveChangesAsync();

        // 2. Secondary Set (Update)
        var updatedPerm = new Permission(writeAction.Id, PermissionOutcome.Allowed, userId: user.Id);
        await permRepo.SetAsync(updatedPerm);
        await unitOfWork.SaveChangesAsync();

        // Assert
        var allPermissions = (await permRepo.GetAllAsync()).ToList();
        allPermissions.Should().HaveCount(1);
        allPermissions.First().Outcome.Should().Be(PermissionOutcome.Allowed);
    }

    [Fact]
    public async Task Should_AllowBothUserIdAndRoleId_Simultaneously()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var tenantRepo = scope.ServiceProvider.GetRequiredService<ITenantRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var tenant = new Tenant("Multi Tenant", "multi-tenant");
        await tenantRepo.AddAsync(tenant);
        await unitOfWork.SaveChangesAsync();

        var userRepo = tenantRepo.GetUserRepository(tenant.Id);
        var user = new User(tenant.Id, "multi-user", "multi@test.com", "hash");
        await userRepo.AddAsync(user);

        var roleRepo = tenantRepo.GetRoleRepository(tenant.Id);
        var role = new Role(tenant.Id, "Manager");
        await roleRepo.AddAsync(role);

        var opRepo = tenantRepo.GetOperationRepository(tenant.Id);
        var op = new Operation(tenant.Id, "Manage", "manage");
        await opRepo.AddAsync(op);

        var appRepo = tenantRepo.GetApplicationRepository(tenant.Id);
        var app = new IamPlatform.Domain.Applications.Application(tenant.Id, "Multi App", "multi-app");
        await appRepo.AddAsync(app);
        await unitOfWork.SaveChangesAsync();

        var resRepo = appRepo.GetResourceRepository(app.Id);
        var resource = new Resource(app.Id, "Dashboard", "dashboard");
        await resRepo.AddAsync(resource);
        await unitOfWork.SaveChangesAsync();

        var actionRepo = resRepo.GetActionRepository(resource.Id);
        var action = new Action(resource.Id, op.Id);
        await actionRepo.AddAsync(action);
        await unitOfWork.SaveChangesAsync();

        var permRepo = actionRepo.GetPermissionRepository(action.Id);

        // Act
        // 1. User-only permission
        await permRepo.SetAsync(new Permission(action.Id, PermissionOutcome.Allowed, userId: user.Id));
        
        // 2. Role-only permission
        await permRepo.SetAsync(new Permission(action.Id, PermissionOutcome.Denied, roleId: role.Id));

        // 3. User + Role permission (The combination)
        await permRepo.SetAsync(new Permission(action.Id, PermissionOutcome.Inherited, userId: user.Id, roleId: role.Id));

        await unitOfWork.SaveChangesAsync();

        // Assert
        var all = (await permRepo.GetAllAsync()).ToList();
        all.Should().HaveCount(3);
    }
}


