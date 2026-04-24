using IamPlatform.Domain.Applications;
using Microsoft.EntityFrameworkCore;
using IamPlatform.Domain.Authorization;
using Action = IamPlatform.Domain.Applications.Action;

namespace IamPlatform.Infrastructure.Persistence.Repositories;

public class ActionRepository : ResourceScopedRepository<Action>, IActionRepository
{
    public ActionRepository(IamPlatformDbContext context, Guid resourceId) : base(context, resourceId)
    {
    }

    public async Task<Action?> GetByOperationAsync(Guid operationId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(a => a.ResourceId == _resourceId && a.OperationId == operationId, cancellationToken);
    }

    public IPermissionRepository GetPermissionRepository(Guid actionId)
    {
        return new PermissionRepository(_context, actionId);
    }
}

