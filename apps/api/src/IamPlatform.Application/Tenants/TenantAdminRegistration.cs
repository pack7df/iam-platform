using IamPlatform.Domain.Common;
using IamPlatform.Domain.Tenants;

namespace IamPlatform.Application.Tenants;

public sealed class TenantAdminRegistration : ITenantAdminRegistration
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IUserRepository _tenantUserRepository;
    private readonly IUnitOfWork _uow;

    public TenantAdminRegistration(
        ITenantRepository tenantRepository,
        IUserRepository tenantUserRepository,
        IUnitOfWork uow)
    {
        _tenantRepository = tenantRepository;
        _tenantUserRepository = tenantUserRepository;
        _uow = uow;
    }

    public async Task<TenantAdminRegistrationResult> RegisterAsync(
        string tenantId,
        string tenantName,
        string tenantAdminId,
        CancellationToken cancellationToken = default)
    {
        var tenant = Tenant.Create(tenantId, tenantName);
        var tenantAdmin = User.Create(tenantAdminId, tenant.Id, UserType.TenantAdmin);

        await _tenantRepository.AddAsync(tenant, cancellationToken);
        await _tenantUserRepository.AddAsync(tenantAdmin, cancellationToken);

        await _uow.SaveChangesAsync(cancellationToken);

        return new TenantAdminRegistrationResult(tenant, tenantAdmin);
    }
}
