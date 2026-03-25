using IamPlatform.Domain.Identity;

namespace IamPlatform.Application.Tenants;

public interface ITenantAdminInvitation
{
    Task<TenantAdminInvitationResult> InviteAsync(
        string invitationId,
        string tenantId,
        string invitedTenantAdminId,
        CancellationToken cancellationToken = default);
}
