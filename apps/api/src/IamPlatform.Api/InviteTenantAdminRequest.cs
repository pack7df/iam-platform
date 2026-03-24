namespace IamPlatform.Api;

public sealed record InviteTenantAdminRequest(string InvitationId, string TenantId, string InvitedTenantAdminId);
