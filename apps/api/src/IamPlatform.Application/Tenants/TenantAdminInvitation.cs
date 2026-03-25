using IamPlatform.Domain.Common;
using IamPlatform.Domain.Identity;

namespace IamPlatform.Application.Tenants;

public sealed class TenantAdminInvitation : ITenantAdminInvitation
{
    private readonly IInvitationRepository _invitationRepository;
    private readonly IUnitOfWork _uow;

    public TenantAdminInvitation(IInvitationRepository invitationRepository, IUnitOfWork uow)
    {
        _invitationRepository = invitationRepository;
        _uow = uow;
    }

    public async Task<TenantAdminInvitationResult> InviteAsync(
        string invitationId,
        string tenantId,
        string invitedTenantAdminId,
        CancellationToken cancellationToken = default)
    {
        var invitation = Invitation.InviteTenantAdmin(invitationId, tenantId, invitedTenantAdminId);
        await _invitationRepository.AddAsync(invitation, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return new TenantAdminInvitationResult(invitation);
    }
}
