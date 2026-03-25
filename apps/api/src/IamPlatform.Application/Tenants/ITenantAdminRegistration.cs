using IamPlatform.Domain.Tenants;

namespace IamPlatform.Application.Tenants;

public interface ITenantAdminRegistration
{
    Task<TenantAdminRegistrationResult> RegisterAsync(
        string tenantId,
        string tenantName,
        string tenantAdminId,
        CancellationToken cancellationToken = default);
}
