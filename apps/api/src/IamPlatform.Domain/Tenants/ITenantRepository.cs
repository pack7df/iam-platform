namespace IamPlatform.Domain.Tenants;

public interface ITenantRepository
{
    Task AddAsync(Tenant tenant, CancellationToken cancellationToken = default);
}
