using IamPlatform.Domain.Tenants;

namespace IamPlatform.Application.Tenants;

public sealed record TenantAdminRegistrationResult(Tenant Tenant, User Admin);
