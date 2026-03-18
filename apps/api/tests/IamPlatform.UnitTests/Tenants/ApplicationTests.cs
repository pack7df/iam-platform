using FluentAssertions;
using IamPlatform.Domain.Tenants;
using Xunit;

namespace IamPlatform.UnitTests.Tenants;

public sealed class ApplicationTests
{
    [Fact]
    public void Create_Should_Set_Id_TenantId_Name_And_Active_Status()
    {
        var application = Application.Create("app-001", "tenant-001", "Admin Portal");

        application.Id.Should().Be("app-001");
        application.TenantId.Should().Be("tenant-001");
        application.Name.Should().Be("Admin Portal");
        application.IsActive.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_Should_Reject_Invalid_Id(string invalidId)
    {
        var act = () => Application.Create(invalidId, "tenant-001", "Admin Portal");

        act.Should().Throw<ArgumentException>()
            .WithMessage("Application id is required.*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_Should_Reject_Invalid_TenantId(string invalidTenantId)
    {
        var act = () => Application.Create("app-001", invalidTenantId, "Admin Portal");

        act.Should().Throw<ArgumentException>()
            .WithMessage("Tenant id is required.*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_Should_Reject_Invalid_Name(string invalidName)
    {
        var act = () => Application.Create("app-001", "tenant-001", invalidName);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Application name is required.*");
    }

    [Fact]
    public void Rename_Should_Update_Name()
    {
        var application = Application.Create("app-001", "tenant-001", "Admin Portal");

        application.Rename("Customer Portal");

        application.Name.Should().Be("Customer Portal");
    }
}
