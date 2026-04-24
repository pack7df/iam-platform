using IamPlatform.Domain.Operations;
using Microsoft.EntityFrameworkCore;

namespace IamPlatform.Infrastructure.Persistence.Repositories;

public class OperationRepository : TenantScopedRepository<Operation>, IOperationRepository
{
    public OperationRepository(IamPlatformDbContext context, Guid tenantId) : base(context, tenantId)
    {
    }

    public async Task<Operation?> GetByKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(o => o.TenantId == _tenantId && o.Key == key, cancellationToken);
    }
}
