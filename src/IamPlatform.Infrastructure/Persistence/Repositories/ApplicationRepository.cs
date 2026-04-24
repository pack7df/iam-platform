using IamPlatform.Domain.Applications;
using Microsoft.EntityFrameworkCore;

namespace IamPlatform.Infrastructure.Persistence.Repositories;

public class ApplicationRepository : TenantScopedRepository<IamPlatform.Domain.Applications.Application>, IApplicationRepository
{
    public ApplicationRepository(IamPlatformDbContext context, Guid tenantId) : base(context, tenantId)
    {
    }

    public async Task<IamPlatform.Domain.Applications.Application?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(a => a.Slug == slug && a.TenantId == _tenantId, cancellationToken);
    }

    public IResourceRepository GetResourceRepository(Guid applicationId)
    {
        return new ResourceRepository(_context, applicationId);
    }
}


