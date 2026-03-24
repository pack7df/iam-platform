using IamPlatform.Domain.Tenants;

namespace IamPlatform.Infrastructure;

internal sealed class InMemoryUserRepository : IUserRepository
{
    private readonly InMemoryTenantStore _store;

    public InMemoryUserRepository(InMemoryTenantStore store)
    {
        _store = store;
    }

     public Task AddAsync(User user, CancellationToken cancellationToken = default)
     {
         _store.Users.Add(user);
         return Task.CompletedTask;
     }

     public Task<User?> GetByIdAsync(string userId, CancellationToken cancellationToken = default)
     {
         var result = _store.Users.FirstOrDefault(u => u.Id == userId);
         return Task.FromResult<User?>(result);
     }
 }
