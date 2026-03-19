using IamPlatform.Domain.Tenants;

namespace IamPlatform.Application.Tenants;

public sealed class TenantAdminRegistration
{
    private readonly ITenantRepository _tenantRepository;
    private readonly ITenantUserRepository _tenantUserRepository;

    public TenantAdminRegistration(
        ITenantRepository tenantRepository,
        ITenantUserRepository tenantUserRepository)
    {
        _tenantRepository = tenantRepository;
        _tenantUserRepository = tenantUserRepository;
    }

    public async Task<TenantAdminRegistrationResult> RegisterAsync(
        string tenantId,
        string tenantName,
        string tenantAdminId,
        CancellationToken cancellationToken = default)
    {
        var tenant = Tenant.Create(tenantId, tenantName);
        var tenantAdmin = TenantUser.Create(tenantAdminId, tenant.Id, TenantUserType.TenantAdmin);

        await _tenantRepository.AddAsync(tenant, cancellationToken);
        await _tenantUserRepository.AddAsync(tenantAdmin, cancellationToken);

        return new TenantAdminRegistrationResult(tenant, tenantAdmin);
    }
}

public sealed record TenantAdminRegistrationResult(Tenant Tenant, TenantUser TenantAdmin);
