using FluentAssertions;
using IamPlatform.Application.Identity;
using IamPlatform.Domain.Identity;
using Xunit;

namespace IamPlatform.UnitTests.Identity;

public sealed class SystemUserInvitationTests
{
    [Fact]
    public async Task InviteAsync_Should_Create_Pending_SystemUser_Invitation()
    {
        var repository = new FakeInvitationRepository();
        var invitationService = new SystemUserInvitation(repository);

        var result = await invitationService.InviteAsync("invite-001", "system-user-002");

        result.Invitation.Id.Should().Be("invite-001");
        result.Invitation.InvitedIdentityId.Should().Be("system-user-002");
        result.Invitation.TargetType.Should().Be(InvitationTargetType.SystemUser);
        result.Invitation.TenantId.Should().BeNull();
        result.Invitation.Status.Should().Be(InvitationStatus.Pending);
        repository.AddedInvitation.Should().NotBeNull();
        repository.AddedInvitation!.Id.Should().Be("invite-001");
    }

    [Theory]
    [InlineData("", "system-user-002", "Invitation id is required.*")]
    [InlineData(" ", "system-user-002", "Invitation id is required.*")]
    [InlineData("invite-001", "", "Invited identity id is required.*")]
    [InlineData("invite-001", " ", "Invited identity id is required.*")]
    public async Task InviteAsync_Should_Reject_Invalid_Input(
        string invitationId,
        string invitedSystemUserId,
        string expectedMessage)
    {
        var repository = new FakeInvitationRepository();
        var invitationService = new SystemUserInvitation(repository);

        var act = () => invitationService.InviteAsync(invitationId, invitedSystemUserId);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage(expectedMessage);
        repository.AddedInvitation.Should().BeNull();
    }

    private sealed class FakeInvitationRepository : IInvitationRepository
    {
        public Invitation? AddedInvitation { get; private set; }

        public Task<Invitation?> GetByIdAsync(string id, CancellationToken cancellationToken = default) => throw new NotImplementedException();

        public Task AddAsync(Invitation invitation, CancellationToken cancellationToken = default)
        {
            AddedInvitation = invitation;
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Invitation invitation, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    }
}
