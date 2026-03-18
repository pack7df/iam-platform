using FluentAssertions;
using IamPlatform.Domain.Tenants;
using Xunit;

namespace IamPlatform.UnitTests.Tenants;

public sealed class RoleTests
{
    [Fact]
    public void Create_Should_Set_Id_TenantId_Name_And_Active_Status()
    {
        var role = Role.Create("role-001", "tenant-001", "Administrators");

        role.Id.Should().Be("role-001");
        role.TenantId.Should().Be("tenant-001");
        role.Name.Should().Be("Administrators");
        role.IsActive.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_Should_Reject_Invalid_Id(string invalidId)
    {
        var act = () => Role.Create(invalidId, "tenant-001", "Administrators");

        act.Should().Throw<ArgumentException>()
            .WithMessage("Role id is required.*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_Should_Reject_Invalid_TenantId(string invalidTenantId)
    {
        var act = () => Role.Create("role-001", invalidTenantId, "Administrators");

        act.Should().Throw<ArgumentException>()
            .WithMessage("Tenant id is required.*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_Should_Reject_Invalid_Name(string invalidName)
    {
        var act = () => Role.Create("role-001", "tenant-001", invalidName);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Role name is required.*");
    }

    [Fact]
    public void Rename_Should_Update_Name()
    {
        var role = Role.Create("role-001", "tenant-001", "Administrators");

        role.Rename("Operators");

        role.Name.Should().Be("Operators");
    }
}
