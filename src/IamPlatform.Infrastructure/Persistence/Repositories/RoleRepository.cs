using IamPlatform.Domain.Tenants;
using Microsoft.EntityFrameworkCore;

namespace IamPlatform.Infrastructure.Persistence.Repositories;

public class RoleRepository : TenantScopedRepository<Role>, IRoleRepository
{
    public RoleRepository(IamPlatformDbContext context, Guid tenantId) : base(context, tenantId)
    {
    }

    public async Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(r => r.TenantId == _tenantId && r.Name == name, cancellationToken);
    }
}
