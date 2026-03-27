using FluentAssertions;
using IamPlatform.Application.Identity;
using IamPlatform.Domain.Identity;
using Xunit;

namespace IamPlatform.UnitTests.Identity;

public sealed class SystemUserInvitationTests
{
    [Fact]
    public async Task Handle_Should_Create_Pending_SystemUser_Invitation()
    {
        var repository = new FakeInvitationRepository();
        var handler = new InviteSystemUserHandler(repository);
        var command = new InviteSystemUserCommand("invite-001", "system-user-002");

        var result = await handler.Handle(command, CancellationToken.None);

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
    public async Task Handle_Should_Reject_Invalid_Input(
        string invitationId,
        string invitedSystemUserId,
        string expectedMessage)
    {
        var repository = new FakeInvitationRepository();
        var handler = new InviteSystemUserHandler(repository);
        var command = new InviteSystemUserCommand(invitationId, invitedSystemUserId);

        var act = () => handler.Handle(command, CancellationToken.None);

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
