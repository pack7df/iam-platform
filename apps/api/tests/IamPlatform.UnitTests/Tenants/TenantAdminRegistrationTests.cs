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
        var tenantUserRepository = new FakeTenantUserRepository();
        var registration = new TenantAdminRegistration(tenantRepository, tenantUserRepository);

        var result = await registration.RegisterAsync("tenant-001", "Acme Corp", "tenant-admin-001");

        result.Tenant.Id.Should().Be("tenant-001");
        result.Tenant.Name.Should().Be("Acme Corp");
        result.TenantAdmin.Id.Should().Be("tenant-admin-001");
        result.TenantAdmin.TenantId.Should().Be("tenant-001");
        result.TenantAdmin.Type.Should().Be(TenantUserType.TenantAdmin);
        tenantRepository.AddedTenant.Should().NotBeNull();
        tenantUserRepository.AddedTenantUser.Should().NotBeNull();
    }

    [Theory]
    [InlineData("", "Acme Corp", "tenant-admin-001", "Tenant id is required.*")]
    [InlineData(" ", "Acme Corp", "tenant-admin-001", "Tenant id is required.*")]
    [InlineData("tenant-001", "", "tenant-admin-001", "Tenant name is required.*")]
    [InlineData("tenant-001", " ", "tenant-admin-001", "Tenant name is required.*")]
    [InlineData("tenant-001", "Acme Corp", "", "Tenant user id is required.*")]
    [InlineData("tenant-001", "Acme Corp", " ", "Tenant user id is required.*")]
    public async Task RegisterAsync_Should_Reject_Invalid_Input(
        string tenantId,
        string tenantName,
        string tenantAdminId,
        string expectedMessage)
    {
        var tenantRepository = new FakeTenantRepository();
        var tenantUserRepository = new FakeTenantUserRepository();
        var registration = new TenantAdminRegistration(tenantRepository, tenantUserRepository);

        var act = () => registration.RegisterAsync(tenantId, tenantName, tenantAdminId);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage(expectedMessage);
        tenantRepository.AddedTenant.Should().BeNull();
        tenantUserRepository.AddedTenantUser.Should().BeNull();
    }

    private sealed class FakeTenantRepository : ITenantRepository
    {
        public Tenant? AddedTenant { get; private set; }

        public Task AddAsync(Tenant tenant, CancellationToken cancellationToken = default)
        {
            AddedTenant = tenant;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeTenantUserRepository : ITenantUserRepository
    {
        public TenantUser? AddedTenantUser { get; private set; }

        public Task AddAsync(TenantUser tenantUser, CancellationToken cancellationToken = default)
        {
            AddedTenantUser = tenantUser;
            return Task.CompletedTask;
        }
    }
}
