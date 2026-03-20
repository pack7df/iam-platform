using FluentAssertions;
using IamPlatform.Domain.Authorization;
using Xunit;

namespace IamPlatform.UnitTests.Authorization;

public sealed class ResourceTests
{
    [Fact]
    public void CreateRoot_Should_Set_ApplicationId_Name_Key_And_Root_Status()
    {
        var resource = Resource.CreateRoot("resource-001", "app-001", "Dashboard", "dashboard");

        resource.Id.Should().Be("resource-001");
        resource.ApplicationId.Should().Be("app-001");
        resource.Name.Should().Be("Dashboard");
        resource.Key.Should().Be("dashboard");
        resource.ParentId.Should().BeNull();
        resource.IsRoot.Should().BeTrue();
        resource.IsActive.Should().BeTrue();
    }

    [Fact]
    public void CreateChild_Should_Set_ParentId_And_NonRoot_Status()
    {
        var resource = Resource.CreateChild("resource-002", "app-001", "Users", "users", "resource-001");

        resource.ParentId.Should().Be("resource-001");
        resource.IsRoot.Should().BeFalse();
        resource.IsActive.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void CreateRoot_Should_Reject_Invalid_Id(string invalidId)
    {
        var act = () => Resource.CreateRoot(invalidId, "app-001", "Dashboard", "dashboard");

        act.Should().Throw<ArgumentException>()
            .WithMessage("Resource id is required.*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void CreateRoot_Should_Reject_Invalid_ApplicationId(string invalidApplicationId)
    {
        var act = () => Resource.CreateRoot("resource-001", invalidApplicationId, "Dashboard", "dashboard");

        act.Should().Throw<ArgumentException>()
            .WithMessage("Application id is required.*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void CreateRoot_Should_Reject_Invalid_Name(string invalidName)
    {
        var act = () => Resource.CreateRoot("resource-001", "app-001", invalidName, "dashboard");

        act.Should().Throw<ArgumentException>()
            .WithMessage("Resource name is required.*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void CreateRoot_Should_Reject_Invalid_Key(string invalidKey)
    {
        var act = () => Resource.CreateRoot("resource-001", "app-001", "Dashboard", invalidKey);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Resource key is required.*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void CreateChild_Should_Reject_Invalid_ParentId(string invalidParentId)
    {
        var act = () => Resource.CreateChild("resource-002", "app-001", "Users", "users", invalidParentId);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Parent resource id is required.*");
    }

    [Fact]
    public void Rename_Should_Update_Name()
    {
        var resource = Resource.CreateRoot("resource-001", "app-001", "Dashboard", "dashboard");

        resource.Rename("Control Panel");

        resource.Name.Should().Be("Control Panel");
    }

    [Fact]
    public void ChangeKey_Should_Update_Key()
    {
        var resource = Resource.CreateRoot("resource-001", "app-001", "Dashboard", "dashboard");

        resource.ChangeKey("control-panel");

        resource.Key.Should().Be("control-panel");
    }

    [Fact]
    public void Deactivate_Should_Set_Inactive_Status()
    {
        var resource = Resource.CreateRoot("resource-001", "app-001", "Dashboard", "dashboard");

        resource.Deactivate();

        resource.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Activate_Should_Set_Active_Status()
    {
        var resource = Resource.CreateRoot("resource-001", "app-001", "Dashboard", "dashboard");
        resource.Deactivate();

        resource.Activate();

        resource.IsActive.Should().BeTrue();
    }
}
