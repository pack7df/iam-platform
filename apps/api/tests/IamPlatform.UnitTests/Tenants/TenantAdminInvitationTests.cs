using FluentAssertions;
using IamPlatform.Application.Tenants;
using IamPlatform.Domain.Identity;
using Xunit;

namespace IamPlatform.UnitTests.Tenants;

public sealed class TenantAdminInvitationTests
{
    [Fact]
    public async Task InviteAsync_Should_Create_Pending_TenantAdmin_Invitation_For_Same_Tenant()
    {
        var repository = new FakeInvitationRepository();
        var uow = new FakeUnitOfWork();
        var invitationService = new TenantAdminInvitation(repository, uow);

        var result = await invitationService.InviteAsync("invite-001", "tenant-001", "tenant-admin-002");

        result.Invitation.Id.Should().Be("invite-001");
        result.Invitation.InvitedIdentityId.Should().Be("tenant-admin-002");
        result.Invitation.TargetType.Should().Be(InvitationTargetType.TenantAdmin);
        result.Invitation.TenantId.Should().Be("tenant-001");
        result.Invitation.Status.Should().Be(InvitationStatus.Pending);
        repository.AddedInvitation.Should().NotBeNull();
        repository.AddedInvitation!.TenantId.Should().Be("tenant-001");
        uow.SaveChangesCalled.Should().BeTrue();
    }

    [Theory]
    [InlineData("", "tenant-001", "tenant-admin-002", "Invitation id is required.*")]
    [InlineData(" ", "tenant-001", "tenant-admin-002", "Invitation id is required.*")]
    [InlineData("invite-001", "", "tenant-admin-002", "Tenant id is required.*")]
    [InlineData("invite-001", " ", "tenant-admin-002", "Tenant id is required.*")]
    [InlineData("invite-001", "tenant-001", "", "Invited identity id is required.*")]
    [InlineData("invite-001", "tenant-001", " ", "Invited identity id is required.*")]
    public async Task InviteAsync_Should_Reject_Invalid_Input(
        string invitationId,
        string tenantId,
        string invitedTenantAdminId,
        string expectedMessage)
    {
        var repository = new FakeInvitationRepository();
        var uow = new FakeUnitOfWork();
        var invitationService = new TenantAdminInvitation(repository, uow);

        var act = () => invitationService.InviteAsync(invitationId, tenantId, invitedTenantAdminId);

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
