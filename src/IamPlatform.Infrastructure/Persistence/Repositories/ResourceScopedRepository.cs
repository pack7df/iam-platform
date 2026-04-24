using IamPlatform.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace IamPlatform.Infrastructure.Persistence.Repositories;

public abstract class ResourceScopedRepository<T> : BaseRepository<T> where T : BaseEntity, IResourceEntity
{
    protected readonly Guid _resourceId;

    protected ResourceScopedRepository(IamPlatformDbContext context, Guid resourceId) : base(context)
    {
        _resourceId = resourceId;
    }

    public override async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await base.GetByIdAsync(id, cancellationToken);
        
        if (entity != null && entity.ResourceId != _resourceId)
            return null;

        return entity;
    }

    public override async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(e => e.ResourceId == _resourceId).ToListAsync(cancellationToken);
    }

    public override async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        entity.ResourceId = _resourceId;
        await base.AddAsync(entity, cancellationToken);
    }
}
