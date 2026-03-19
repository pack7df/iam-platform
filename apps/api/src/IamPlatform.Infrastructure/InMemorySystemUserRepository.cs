using IamPlatform.Domain.Identity;

namespace IamPlatform.Infrastructure;

internal sealed class InMemorySystemUserRepository : ISystemUserRepository
{
    private readonly InMemoryIdentityStore _store;

    public InMemorySystemUserRepository(InMemoryIdentityStore store)
    {
        _store = store;
    }

    public Task<bool> ExistsAnySystemUserAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_store.SystemUsers.Count > 0);
    }

    public Task AddAsync(SystemUser systemUser, CancellationToken cancellationToken = default)
    {
        _store.SystemUsers.Add(systemUser);
        return Task.CompletedTask;
    }
}
