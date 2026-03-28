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
        tenantUser.Status.Should().Be(UserStatus.PendingVerification);
        tenantUser.IsActive.Should().BeFalse();
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
            .WithMessage("Tenant id is required for non-system users.*");
    }

    [Fact]
    public void ChangeType_Should_Update_UserType()
    {
        var tenantUser = User.Create("user-001", "tenant-001", UserType.EndUser);

        tenantUser.ChangeType(UserType.ServiceAdmin);

        tenantUser.Type.Should().Be(UserType.ServiceAdmin);
    }

    [Fact]
    public void UpdateStatus_Should_Change_Status()
    {
        var tenantUser = User.Create("user-001", "tenant-001", UserType.EndUser);

        tenantUser.UpdateStatus(UserStatus.Suspended);

        tenantUser.Status.Should().Be(UserStatus.Suspended);
        tenantUser.IsActive.Should().BeFalse();
    }

    [Fact]
    public void UpdateStatus_To_Active_Should_Make_User_Active()
    {
        var tenantUser = User.Create("user-001", "tenant-001", UserType.EndUser);
        
        tenantUser.UpdateStatus(UserStatus.Active);

        tenantUser.IsActive.Should().BeTrue();
    }

    [Fact]
    public void SetPassword_Should_Set_Hash_And_Salt()
    {
        var user = User.Create("user-001", "tenant-001", UserType.EndUser);

        user.SetPassword("hashed-pwd", "random-salt");

        user.PasswordHash.Should().Be("hashed-pwd");
        user.Salt.Should().Be("random-salt");
    }
}
