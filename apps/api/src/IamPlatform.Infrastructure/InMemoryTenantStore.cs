using IamPlatform.Domain.Tenants;

namespace IamPlatform.Infrastructure;

internal sealed class InMemoryTenantStore
{
    public List<Tenant> Tenants { get; } = new();

    public List<User> Users { get; } = new();
}
