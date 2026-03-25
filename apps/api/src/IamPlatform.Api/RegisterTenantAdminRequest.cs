namespace IamPlatform.Api;

public sealed record RegisterTenantAdminRequest(string TenantId, string TenantName, string TenantAdminId);
