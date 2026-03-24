namespace IamPlatform.Application.Tenants;

public sealed record RegisterTenantAdminRequest(string TenantId, string TenantName, string TenantAdminId);
