using IamPlatform.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace IamPlatform.Infrastructure.Persistence.Repositories;

public class UserRepository : TenantScopedRepository<User>, IUserRepository
{
    public UserRepository(IamPlatformDbContext context, Guid tenantId) : base(context, tenantId)
    {
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Email == email && u.TenantId == _tenantId, cancellationToken);
    }
}
