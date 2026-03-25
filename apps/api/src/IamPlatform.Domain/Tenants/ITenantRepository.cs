namespace IamPlatform.Domain.Tenants;

public interface ITenantRepository
{
    Task<Tenant?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task AddAsync(Tenant tenant, CancellationToken cancellationToken = default);
    Task UpdateAsync(Tenant tenant, CancellationToken cancellationToken = default);
}
