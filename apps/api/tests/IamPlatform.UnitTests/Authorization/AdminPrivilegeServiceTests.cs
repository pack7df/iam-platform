using FluentAssertions;
using IamPlatform.Domain.Authorization;
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
        var systemUser = User.CreateSystemUser(userId);
        var repo = new FakeUserRepository(new[] { systemUser });
        var service = new AdminPrivilegeService(repo);

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
        var repo = new FakeUserRepository(Array.Empty<User>());
        var service = new AdminPrivilegeService(repo);

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
        var repo = new FakeUserRepository(new[] { tenantAdmin });
        var service = new AdminPrivilegeService(repo);

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
        var repo = new FakeUserRepository(new[] { endUser });
        var service = new AdminPrivilegeService(repo);

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
        var repo = new FakeUserRepository(new[] { tenantAdmin });
        var service = new AdminPrivilegeService(repo);

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
        var repo = new FakeUserRepository(Array.Empty<User>());
        var service = new AdminPrivilegeService(repo);

        // Act
        var result = await service.HasTenantAdminPrivilegesAsync(tenantId, userId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task HasTenantAdminPrivilegesAsync_Should_Return_True_For_SystemUser()
    {
        // Arrange
        var tenantId = "any-tenant";
        var userId = "sys-user-001";
        var systemUser = User.CreateSystemUser(userId);
        var repo = new FakeUserRepository(new[] { systemUser });
        var service = new AdminPrivilegeService(repo);

        // Act
        var result = await service.HasTenantAdminPrivilegesAsync(tenantId, userId);

        // Assert
        result.Should().BeTrue(); // SystemUser has global admin privileges
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        private readonly List<User> _users;

        public FakeUserRepository(IEnumerable<User> users)
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

        public Task UpdateAsync(User user, CancellationToken cancellationToken = default) => throw new NotImplementedException();

        public Task<IReadOnlyCollection<User>> GetByTenantIdAsync(string tenantId, CancellationToken cancellationToken = default)
        {
            var result = _users.Where(u => u.TenantId == tenantId).ToList();
            return Task.FromResult<IReadOnlyCollection<User>>(result);
        }

        public Task<bool> ExistsByTypeAsync(UserType type, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_users.Any(u => u.Type == type));
        }
    }
}
