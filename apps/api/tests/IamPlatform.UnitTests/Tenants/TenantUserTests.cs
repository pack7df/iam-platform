using FluentAssertions;
using IamPlatform.Domain.Tenants;
using Xunit;

namespace IamPlatform.UnitTests.Tenants;

public sealed class UserTests
{
    [Fact]
    public void Create_Should_Set_Id_TenantId_And_UserType()
    {
        var tenantUser = User.Create("user-001", "tenant-001", UserType.TenantAdmin);

        tenantUser.Id.Should().Be("user-001");
        tenantUser.TenantId.Should().Be("tenant-001");
        tenantUser.Type.Should().Be(UserType.TenantAdmin);
        tenantUser.IsActive.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_Should_Reject_Invalid_Id(string invalidId)
    {
        var act = () => User.Create(invalidId, "tenant-001", UserType.EndUser);

        act.Should().Throw<ArgumentException>()
            .WithMessage("User id is required.*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_Should_Reject_Invalid_TenantId(string invalidTenantId)
    {
        var act = () => User.Create("user-001", invalidTenantId, UserType.EndUser);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Tenant id is required.*");
    }

    [Fact]
    public void ChangeType_Should_Update_UserType()
    {
        var tenantUser = User.Create("user-001", "tenant-001", UserType.EndUser);

        tenantUser.ChangeType(UserType.ServiceAdmin);

        tenantUser.Type.Should().Be(UserType.ServiceAdmin);
    }

    [Fact]
    public void Deactivate_Should_Set_Inactive_Status()
    {
        var tenantUser = User.Create("user-001", "tenant-001", UserType.EndUser);

        tenantUser.Deactivate();

        tenantUser.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Activate_Should_Set_Active_Status()
    {
        var tenantUser = User.Create("user-001", "tenant-001", UserType.EndUser);
        tenantUser.Deactivate();

        tenantUser.Activate();

        tenantUser.IsActive.Should().BeTrue();
    }
}
