using IamPlatform.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace IamPlatform.Infrastructure.Persistence.Repositories;

public abstract class TenantScopedRepository<T> : BaseRepository<T> where T : BaseEntity, ITenantEntity
{
    protected readonly Guid _tenantId;

    protected TenantScopedRepository(IamPlatformDbContext context, Guid tenantId) : base(context)
    {
        _tenantId = tenantId;
    }

    public override async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await base.GetByIdAsync(id, cancellationToken);
        
        if (entity != null && entity.TenantId != _tenantId)
            return null;

        return entity;
    }

    public override async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(e => e.TenantId == _tenantId).ToListAsync(cancellationToken);
    }

    public override async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        // Enforce the tenant context on save without reflection
        entity.TenantId = _tenantId;

        await base.AddAsync(entity, cancellationToken);
    }

}
