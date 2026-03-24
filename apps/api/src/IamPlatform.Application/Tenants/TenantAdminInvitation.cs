using IamPlatform.Domain.Identity;

namespace IamPlatform.Application.Tenants;

public sealed class TenantAdminInvitation
{
    private readonly IInvitationRepository _invitationRepository;

    public TenantAdminInvitation(IInvitationRepository invitationRepository)
    {
        _invitationRepository = invitationRepository;
    }

    public async Task<TenantAdminInvitationResult> InviteAsync(
        string invitationId,
        string tenantId,
        string invitedTenantAdminId,
        CancellationToken cancellationToken = default)
    {
        var invitation = Invitation.InviteTenantAdmin(invitationId, tenantId, invitedTenantAdminId);
        await _invitationRepository.AddAsync(invitation, cancellationToken);

        return new TenantAdminInvitationResult(invitation);
    }
}
