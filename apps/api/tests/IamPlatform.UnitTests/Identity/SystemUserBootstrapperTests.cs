using FluentAssertions;
using IamPlatform.Domain.Identity;
using Xunit;

namespace IamPlatform.UnitTests.Identity;

public sealed class SystemUserBootstrapperTests
{
    [Fact]
    public void Bootstrap_Should_Create_First_SystemUser_When_None_Exists()
    {
        var bootstrapper = new SystemUserBootstrapper();

        var systemUser = bootstrapper.BootstrapFirstSystemUser(Array.Empty<SystemUser>(), "system-user-001");

        systemUser.Id.Should().Be("system-user-001");
        systemUser.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Bootstrap_Should_Reject_When_A_SystemUser_Already_Exists()
    {
        var bootstrapper = new SystemUserBootstrapper();
        var existingUsers = new[] { SystemUser.Create("existing-system-user") };

        var act = () => bootstrapper.BootstrapFirstSystemUser(existingUsers, "system-user-001");

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("The first system user has already been bootstrapped.");
    }
}
