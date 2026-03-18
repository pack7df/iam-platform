using FluentAssertions;
using IamPlatform.Domain.Tenants;
using Xunit;

namespace IamPlatform.UnitTests.Tenants;

public sealed class TenantTests
{
    [Fact]
    public void Create_Should_Set_Id_Name_And_Active_Status()
    {
        var tenant = Tenant.Create("tenant-001", "Acme Corp");

        tenant.Id.Should().Be("tenant-001");
        tenant.Name.Should().Be("Acme Corp");
        tenant.IsActive.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_Should_Reject_Invalid_Id(string invalidId)
    {
        var act = () => Tenant.Create(invalidId, "Acme Corp");

        act.Should().Throw<ArgumentException>()
            .WithMessage("Tenant id is required.*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_Should_Reject_Invalid_Name(string invalidName)
    {
        var act = () => Tenant.Create("tenant-001", invalidName);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Tenant name is required.*");
    }

    [Fact]
    public void Rename_Should_Update_Name()
    {
        var tenant = Tenant.Create("tenant-001", "Acme Corp");

        tenant.Rename("Acme Platform");

        tenant.Name.Should().Be("Acme Platform");
    }

    [Fact]
    public void Deactivate_Should_Set_Inactive_Status()
    {
        var tenant = Tenant.Create("tenant-001", "Acme Corp");

        tenant.Deactivate();

        tenant.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Activate_Should_Set_Active_Status()
    {
        var tenant = Tenant.Create("tenant-001", "Acme Corp");
        tenant.Deactivate();

        tenant.Activate();

        tenant.IsActive.Should().BeTrue();
    }
}
