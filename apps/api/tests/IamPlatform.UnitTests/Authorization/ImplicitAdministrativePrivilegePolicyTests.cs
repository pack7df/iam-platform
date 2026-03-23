using FluentAssertions;
using IamPlatform.Domain.Authorization;
using IamPlatform.Domain.Identity;
using IamPlatform.Domain.Tenants;
using Xunit;

namespace IamPlatform.UnitTests.Authorization;

public sealed class ImplicitAdministrativePrivilegePolicyTests
{
    private readonly ImplicitAdministrativePrivilegePolicy _policy = new();

    [Fact]
    public void CanManagePlatform_Should_Allow_Active_SystemUser()
    {
        var systemUser = SystemUser.Create("system-user-001");

        var result = _policy.CanManagePlatform(systemUser);

        result.Should().BeTrue();
    }

    [Fact]
    public void CanManagePlatform_Should_Reject_Inactive_SystemUser()
    {
        var systemUser = SystemUser.Create("system-user-001");
        systemUser.Deactivate();

        var result = _policy.CanManagePlatform(systemUser);

        result.Should().BeFalse();
    }

    [Fact]
    public void CanManageTenant_Should_Allow_Active_TenantAdmin_For_Same_Tenant()
    {
        var tenantAdmin = User.Create("tenant-user-001", "tenant-001", UserType.TenantAdmin);

        var result = _policy.CanManageTenant(tenantAdmin, "tenant-001");

        result.Should().BeTrue();
    }

    [Fact]
    public void CanManageTenant_Should_Reject_When_TenantAdmin_Is_Inactive()
    {
        var tenantAdmin = User.Create("tenant-user-001", "tenant-001", UserType.TenantAdmin);
        tenantAdmin.Deactivate();

        var result = _policy.CanManageTenant(tenantAdmin, "tenant-001");

        result.Should().BeFalse();
    }

    [Fact]
    public void CanManageTenant_Should_Reject_When_TenantAdmin_Belongs_To_Different_Tenant()
    {
        var tenantAdmin = User.Create("tenant-user-001", "tenant-001", UserType.TenantAdmin);

        var result = _policy.CanManageTenant(tenantAdmin, "tenant-002");

        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(UserType.EndUser)]
    [InlineData(UserType.ServiceAdmin)]
    public void CanManageTenant_Should_Reject_Non_TenantAdmin_Types(UserType userType)
    {
        var tenantUser = User.Create("tenant-user-001", "tenant-001", userType);

        var result = _policy.CanManageTenant(tenantUser, "tenant-001");

        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void CanManageTenant_Should_Reject_Invalid_TenantId(string invalidTenantId)
    {
        var tenantAdmin = User.Create("tenant-user-001", "tenant-001", UserType.TenantAdmin);

        var act = () => _policy.CanManageTenant(tenantAdmin, invalidTenantId);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Tenant id is required.*");
    }
}
