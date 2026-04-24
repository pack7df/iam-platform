using IamPlatform.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace IamPlatform.Infrastructure.Persistence.Repositories;

public abstract class ApplicationScopedRepository<T> : BaseRepository<T> where T : BaseEntity, IApplicationEntity
{
    protected readonly Guid _applicationId;

    protected ApplicationScopedRepository(IamPlatformDbContext context, Guid applicationId) : base(context)
    {
        _applicationId = applicationId;
    }

    public override async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await base.GetByIdAsync(id, cancellationToken);
        
        if (entity != null && entity.ApplicationId != _applicationId)
            return null;

        return entity;
    }

    public override async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(e => e.ApplicationId == _applicationId).ToListAsync(cancellationToken);
    }

    public override async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        entity.ApplicationId = _applicationId;
        await base.AddAsync(entity, cancellationToken);
    }
}
