namespace IamPlatform.Application.Tenants;

public sealed record InviteTenantAdminRequest(string InvitationId, string TenantId, string InvitedTenantAdminId);
