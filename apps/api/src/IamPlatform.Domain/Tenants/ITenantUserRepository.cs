namespace IamPlatform.Domain.Tenants;

public interface ITenantUserRepository
{
    Task AddAsync(TenantUser tenantUser, CancellationToken cancellationToken = default);
}
