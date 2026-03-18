using FluentAssertions;
using IamPlatform.Domain.Identity;
using Xunit;

namespace IamPlatform.UnitTests.Identity;

public sealed class InvitationTests
{
    [Fact]
    public void InviteSystemUser_Should_Create_Pending_Global_Invitation()
    {
        var invitation = Invitation.InviteSystemUser("invite-001", "system-user-001");

        invitation.Id.Should().Be("invite-001");
        invitation.InvitedIdentityId.Should().Be("system-user-001");
        invitation.TargetType.Should().Be(InvitationTargetType.SystemUser);
        invitation.TenantId.Should().BeNull();
        invitation.Status.Should().Be(InvitationStatus.Pending);
    }

    [Fact]
    public void InviteTenantAdmin_Should_Create_Pending_Tenant_Invitation()
    {
        var invitation = Invitation.InviteTenantAdmin("invite-001", "tenant-001", "tenant-admin-001");

        invitation.Id.Should().Be("invite-001");
        invitation.InvitedIdentityId.Should().Be("tenant-admin-001");
        invitation.TargetType.Should().Be(InvitationTargetType.TenantAdmin);
        invitation.TenantId.Should().Be("tenant-001");
        invitation.Status.Should().Be(InvitationStatus.Pending);
    }

    [Fact]
    public void InviteTenantAdmin_Should_Reject_Missing_TenantId()
    {
        var act = () => Invitation.InviteTenantAdmin("invite-001", " ", "tenant-admin-001");

        act.Should().Throw<ArgumentException>()
            .WithMessage("Tenant id is required.*");
    }

    [Fact]
    public void Accept_Should_Set_Status_To_Accepted()
    {
        var invitation = Invitation.InviteSystemUser("invite-001", "system-user-001");

        invitation.Accept();

        invitation.Status.Should().Be(InvitationStatus.Accepted);
    }

    [Fact]
    public void Accept_Should_Reject_When_Invitation_Is_Not_Pending()
    {
        var invitation = Invitation.InviteSystemUser("invite-001", "system-user-001");
        invitation.Accept();

        var act = () => invitation.Accept();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Only pending invitations can be accepted.");
    }
}
