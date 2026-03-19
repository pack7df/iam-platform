using IamPlatform.Domain.Tenants;

namespace IamPlatform.Infrastructure;

internal sealed class InMemoryTenantRepository : ITenantRepository
{
    private readonly InMemoryTenantStore _store;

    public InMemoryTenantRepository(InMemoryTenantStore store)
    {
        _store = store;
    }

    public Task AddAsync(Tenant tenant, CancellationToken cancellationToken = default)
    {
        _store.Tenants.Add(tenant);
        return Task.CompletedTask;
    }
}
