using FluentAssertions;
using IamPlatform.Domain.Identity;
using Xunit;

namespace IamPlatform.UnitTests.Identity;

public sealed class SystemUserTests
{
    [Fact]
    public void Create_Should_Set_Id_And_Active_Status()
    {
        var systemUser = SystemUser.Create("system-user-001");

        systemUser.Id.Should().Be("system-user-001");
        systemUser.IsActive.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_Should_Reject_Invalid_Id(string invalidId)
    {
        var act = () => SystemUser.Create(invalidId);

        act.Should().Throw<ArgumentException>()
            .WithMessage("System user id is required.*");
    }

    [Fact]
    public void Deactivate_Should_Set_Inactive_Status()
    {
        var systemUser = SystemUser.Create("system-user-001");

        systemUser.Deactivate();

        systemUser.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Activate_Should_Set_Active_Status()
    {
        var systemUser = SystemUser.Create("system-user-001");
        systemUser.Deactivate();

        systemUser.Activate();

        systemUser.IsActive.Should().BeTrue();
    }
}
