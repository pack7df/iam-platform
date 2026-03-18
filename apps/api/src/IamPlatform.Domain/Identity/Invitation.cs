using IamPlatform.Domain.Primitives;

namespace IamPlatform.Domain.Identity;

public sealed class Invitation
{
    private Invitation(string id, string invitedIdentityId, InvitationTargetType targetType, string? tenantId, InvitationStatus status)
    {
        Id = Guard.Required(id, nameof(id), "Invitation id is required.");
        InvitedIdentityId = Guard.Required(invitedIdentityId, nameof(invitedIdentityId), "Invited identity id is required.");
        TargetType = targetType;
        TenantId = tenantId;
        Status = status;
    }

    public string Id { get; }

    public string InvitedIdentityId { get; }

    public InvitationTargetType TargetType { get; }

    public string? TenantId { get; }

    public InvitationStatus Status { get; private set; }

    public static Invitation InviteSystemUser(string id, string invitedIdentityId)
    {
        return new Invitation(id, invitedIdentityId, InvitationTargetType.SystemUser, null, InvitationStatus.Pending);
    }

    public static Invitation InviteTenantAdmin(string id, string tenantId, string invitedIdentityId)
    {
        return new Invitation(
            id,
            invitedIdentityId,
            InvitationTargetType.TenantAdmin,
            Guard.Required(tenantId, nameof(tenantId), "Tenant id is required."),
            InvitationStatus.Pending);
    }

    public void Accept()
    {
        if (Status != InvitationStatus.Pending)
        {
            throw new InvalidOperationException("Only pending invitations can be accepted.");
        }

        Status = InvitationStatus.Accepted;
    }
}
