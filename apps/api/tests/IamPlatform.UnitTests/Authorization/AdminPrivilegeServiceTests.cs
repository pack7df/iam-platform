using FluentAssertions;
using IamPlatform.Domain.Authorization;
using IamPlatform.Domain.Identity;
using IamPlatform.Domain.Tenants;
using Xunit;

namespace IamPlatform.UnitTests.Authorization;

public sealed class AdminPrivilegeServiceTests
{
    [Fact]
    public async Task HasGlobalAdminPrivilegesAsync_Should_Return_True_When_SystemUser_Exists()
    {
        // Arrange
        var userId = "sys-user-001";
        var systemUser = SystemUser.Create(userId);
        var repo = new FakeSystemUserRepository(new[] { systemUser });
        var service = new AdminPrivilegeService(repo, new FakeTenantUserRepository(Array.Empty<User>()));

        // Act
        var result = await service.HasGlobalAdminPrivilegesAsync(userId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task HasGlobalAdminPrivilegesAsync_Should_Return_False_When_SystemUser_Not_Found()
    {
        // Arrange
        var userId = "non-existent";
        var repo = new FakeSystemUserRepository(Array.Empty<SystemUser>());
        var service = new AdminPrivilegeService(repo, new FakeTenantUserRepository(Array.Empty<User>()));

        // Act
        var result = await service.HasGlobalAdminPrivilegesAsync(userId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task HasTenantAdminPrivilegesAsync_Should_Return_True_When_User_Is_TenantAdmin_Of_Tenant()
    {
        // Arrange
        var tenantId = "tenant-001";
        var userId = "tenant-admin-001";
        var tenantAdmin = User.Create(userId, tenantId, UserType.TenantAdmin);
        var repo = new FakeTenantUserRepository(new[] { tenantAdmin });
        var service = new AdminPrivilegeService(new FakeSystemUserRepository(Array.Empty<SystemUser>()), repo);

        // Act
        var result = await service.HasTenantAdminPrivilegesAsync(tenantId, userId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task HasTenantAdminPrivilegesAsync_Should_Return_False_When_User_Is_Not_TenantAdmin()
    {
        // Arrange
        var tenantId = "tenant-001";
        var userId = "end-user-001";
        var endUser = User.Create(userId, tenantId, UserType.EndUser);
        var repo = new FakeTenantUserRepository(new[] { endUser });
        var service = new AdminPrivilegeService(new FakeSystemUserRepository(Array.Empty<SystemUser>()), repo);

        // Act
        var result = await service.HasTenantAdminPrivilegesAsync(tenantId, userId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task HasTenantAdminPrivilegesAsync_Should_Return_False_When_User_BelongsTo_Different_Tenant()
    {
        // Arrange
        var tenantId = "tenant-001";
        var userId = "tenant-admin-002";
        var tenantAdmin = User.Create(userId, "tenant-002", UserType.TenantAdmin);
        var repo = new FakeTenantUserRepository(new[] { tenantAdmin });
        var service = new AdminPrivilegeService(new FakeSystemUserRepository(Array.Empty<SystemUser>()), repo);

        // Act
        var result = await service.HasTenantAdminPrivilegesAsync(tenantId, userId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task HasTenantAdminPrivilegesAsync_Should_Return_False_When_User_Not_Found()
    {
        // Arrange
        var tenantId = "tenant-001";
        var userId = "non-existent";
        var repo = new FakeTenantUserRepository(Array.Empty<User>());
        var service = new AdminPrivilegeService(new FakeSystemUserRepository(Array.Empty<SystemUser>()), repo);

        // Act
        var result = await service.HasTenantAdminPrivilegesAsync(tenantId, userId);

        // Assert
        result.Should().BeFalse();
    }

    private sealed class FakeSystemUserRepository : ISystemUserRepository
    {
        private readonly List<SystemUser> _users;

        public FakeSystemUserRepository(IEnumerable<SystemUser> users)
        {
            _users = users.ToList();
        }

        public Task<SystemUser?> GetByIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            var result = _users.FirstOrDefault(u => u.Id == userId);
            return Task.FromResult<SystemUser?>(result);
        }

        public Task<bool> ExistsAnySystemUserAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_users.Any());
        }

        public Task AddAsync(SystemUser systemUser, CancellationToken cancellationToken = default)
        {
            _users.Add(systemUser);
            return Task.CompletedTask;
        }
    }

    private sealed class FakeTenantUserRepository : IUserRepository
    {
        private readonly List<User> _users;

        public FakeTenantUserRepository(IEnumerable<User> users)
        {
            _users = users.ToList();
        }

        public Task<User?> GetByIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            var result = _users.FirstOrDefault(u => u.Id == userId);
            return Task.FromResult<User?>(result);
        }

        public Task AddAsync(User user, CancellationToken cancellationToken = default)
        {
            _users.Add(user);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyCollection<User>> GetByTenantIdAsync(string tenantId, CancellationToken cancellationToken = default)
        {
            var result = _users.Where(u => u.TenantId == tenantId).ToList();
            return Task.FromResult<IReadOnlyCollection<User>>(result);
        }
    }
}
