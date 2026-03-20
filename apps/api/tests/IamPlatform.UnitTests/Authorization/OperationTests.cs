using FluentAssertions;
using IamPlatform.Domain.Authorization;
using Xunit;

namespace IamPlatform.UnitTests.Authorization;

public sealed class OperationTests
{
    [Fact]
    public void Create_Should_Set_Id_ApplicationId_Key_Name_And_Active_Status()
    {
        var operation = Operation.Create("operation-001", "app-001", "read", "Read");

        operation.Id.Should().Be("operation-001");
        operation.ApplicationId.Should().Be("app-001");
        operation.Key.Should().Be("read");
        operation.Name.Should().Be("Read");
        operation.IsActive.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_Should_Reject_Invalid_Id(string invalidId)
    {
        var act = () => Operation.Create(invalidId, "app-001", "read", "Read");

        act.Should().Throw<ArgumentException>()
            .WithMessage("Operation id is required.*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_Should_Reject_Invalid_ApplicationId(string invalidApplicationId)
    {
        var act = () => Operation.Create("operation-001", invalidApplicationId, "read", "Read");

        act.Should().Throw<ArgumentException>()
            .WithMessage("Application id is required.*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_Should_Reject_Invalid_Key(string invalidKey)
    {
        var act = () => Operation.Create("operation-001", "app-001", invalidKey, "Read");

        act.Should().Throw<ArgumentException>()
            .WithMessage("Operation key is required.*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_Should_Reject_Invalid_Name(string invalidName)
    {
        var act = () => Operation.Create("operation-001", "app-001", "read", invalidName);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Operation name is required.*");
    }

    [Fact]
    public void Rename_Should_Update_Name()
    {
        var operation = Operation.Create("operation-001", "app-001", "read", "Read");

        operation.Rename("View");

        operation.Name.Should().Be("View");
    }

    [Fact]
    public void ChangeKey_Should_Update_Key()
    {
        var operation = Operation.Create("operation-001", "app-001", "read", "Read");

        operation.ChangeKey("view");

        operation.Key.Should().Be("view");
    }

    [Fact]
    public void Deactivate_Should_Set_Inactive_Status()
    {
        var operation = Operation.Create("operation-001", "app-001", "read", "Read");

        operation.Deactivate();

        operation.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Activate_Should_Set_Active_Status()
    {
        var operation = Operation.Create("operation-001", "app-001", "read", "Read");
        operation.Deactivate();

        operation.Activate();

        operation.IsActive.Should().BeTrue();
    }
}
