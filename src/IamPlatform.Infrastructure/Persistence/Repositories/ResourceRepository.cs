using IamPlatform.Domain.Applications;
using Microsoft.EntityFrameworkCore;

namespace IamPlatform.Infrastructure.Persistence.Repositories;

public class ResourceRepository : ApplicationScopedRepository<Resource>, IResourceRepository
{
    public ResourceRepository(IamPlatformDbContext context, Guid applicationId) : base(context, applicationId)
    {
    }

    public async Task<Resource?> GetByKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(r => r.ApplicationId == _applicationId && r.Key == key, cancellationToken);
    }

    public IActionRepository GetActionRepository(Guid resourceId)
    {
        return new ActionRepository(_context, resourceId);
    }
}


