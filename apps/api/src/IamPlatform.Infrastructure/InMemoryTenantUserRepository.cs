using IamPlatform.Domain.Tenants;

namespace IamPlatform.Infrastructure;

internal sealed class InMemoryTenantUserRepository : ITenantUserRepository
{
    private readonly InMemoryTenantStore _store;

    public InMemoryTenantUserRepository(InMemoryTenantStore store)
    {
        _store = store;
    }

    public Task AddAsync(TenantUser tenantUser, CancellationToken cancellationToken = default)
    {
        _store.TenantUsers.Add(tenantUser);
        return Task.CompletedTask;
    }
}
