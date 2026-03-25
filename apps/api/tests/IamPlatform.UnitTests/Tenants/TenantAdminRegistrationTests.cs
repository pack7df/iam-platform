using FluentAssertions;
using IamPlatform.Application.Tenants;
using IamPlatform.Domain.Tenants;
using Xunit;

namespace IamPlatform.UnitTests.Tenants;

public sealed class TenantAdminRegistrationTests
{
    [Fact]
    public async Task RegisterAsync_Should_Create_Tenant_And_TenantAdmin()
    {
        var tenantRepository = new FakeTenantRepository();
        var userRepository = new FakeUserRepository();
        var uow = new FakeUnitOfWork();
        var registration = new TenantAdminRegistration(tenantRepository, userRepository, uow);

        var result = await registration.RegisterAsync("tenant-001", "Acme Corp", "tenant-admin-001");

        result.Tenant.Id.Should().Be("tenant-001");
        result.Tenant.Name.Should().Be("Acme Corp");
        result.TenantAdmin.Id.Should().Be("tenant-admin-001");
        result.TenantAdmin.TenantId.Should().Be("tenant-001");
        result.TenantAdmin.Type.Should().Be(UserType.TenantAdmin);
        tenantRepository.AddedTenant.Should().NotBeNull();
        userRepository.AddedUser.Should().NotBeNull();
        uow.SaveChangesCalled.Should().BeTrue();
    }

    [Theory]
    [InlineData("", "Acme Corp", "tenant-admin-001", "Tenant id is required.*")]
    [InlineData(" ", "Acme Corp", "tenant-admin-001", "Tenant id is required.*")]
    [InlineData("tenant-001", "", "tenant-admin-001", "Tenant name is required.*")]
    [InlineData("tenant-001", " ", "tenant-admin-001", "Tenant name is required.*")]
    [InlineData("tenant-001", "Acme Corp", "", "User id is required.*")]
    [InlineData("tenant-001", "Acme Corp", " ", "User id is required.*")]
    public async Task RegisterAsync_Should_Reject_Invalid_Input(
        string tenantId,
        string tenantName,
        string tenantAdminId,
        string expectedMessage)
    {
        var tenantRepository = new FakeTenantRepository();
        var userRepository = new FakeUserRepository();
        var uow = new FakeUnitOfWork();
        var registration = new TenantAdminRegistration(tenantRepository, userRepository, uow);

        var act = () => registration.RegisterAsync(tenantId, tenantName, tenantAdminId);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage(expectedMessage);
        tenantRepository.AddedTenant.Should().BeNull();
        userRepository.AddedUser.Should().BeNull();
    }

    private sealed class FakeTenantRepository : ITenantRepository
    {
        public Tenant? AddedTenant { get; private set; }

        public Task<Tenant?> GetByIdAsync(string id, CancellationToken cancellationToken = default) => throw new NotImplementedException();

        public Task AddAsync(Tenant tenant, CancellationToken cancellationToken = default)
        {
            AddedTenant = tenant;
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Tenant tenant, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    }

      private sealed class FakeUserRepository : IUserRepository
      {
          public User? AddedUser { get; private set; }

          public Task<User?> GetByIdAsync(string userId, CancellationToken cancellationToken = default)
          {
              // Not needed for current tests
              return Task.FromResult<User?>(null);
          }

          public Task AddAsync(User user, CancellationToken cancellationToken = default)
          {
              AddedUser = user;
              return Task.CompletedTask;
          }

          public Task UpdateAsync(User user, CancellationToken cancellationToken = default) => throw new NotImplementedException();

          public Task<IReadOnlyCollection<User>> GetByTenantIdAsync(string tenantId, CancellationToken cancellationToken = default) => throw new NotImplementedException();

          public Task<bool> ExistsByTypeAsync(UserType type, CancellationToken cancellationToken = default)
          {
              // Not needed for current tests
              return Task.FromResult(false);
          }
      }
}
