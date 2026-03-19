using IamPlatform.Domain.Identity;

namespace IamPlatform.Application.Identity;

public sealed class SystemUserInvitation
{
    private readonly IInvitationRepository _invitationRepository;

    public SystemUserInvitation(IInvitationRepository invitationRepository)
    {
        _invitationRepository = invitationRepository;
    }

    public async Task<SystemUserInvitationResult> InviteAsync(
        string invitationId,
        string invitedSystemUserId,
        CancellationToken cancellationToken = default)
    {
        var invitation = Invitation.InviteSystemUser(invitationId, invitedSystemUserId);
        await _invitationRepository.AddAsync(invitation, cancellationToken);

        return new SystemUserInvitationResult(invitation);
    }
}

public sealed record SystemUserInvitationResult(Invitation Invitation);
