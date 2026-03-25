using IamPlatform.Domain.Identity;

namespace IamPlatform.Application.Identity;

public interface ISystemUserInvitation
{
    Task<SystemUserInvitationResult> InviteAsync(
        string invitationId,
        string invitedSystemUserId,
        CancellationToken cancellationToken = default);
}
