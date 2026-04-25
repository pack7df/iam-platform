using IamPlatform.Domain.Authorization;
using Microsoft.EntityFrameworkCore;

namespace IamPlatform.Infrastructure.Persistence.Repositories;

public class PermissionRepository : BaseRepository<Permission>, IPermissionRepository
{
    private readonly Guid _actionId;

    public PermissionRepository(IamPlatformDbContext context, Guid actionId) : base(context)
    {
        _actionId = actionId;
    }

    public async Task<IEnumerable<Permission>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.ActionId == _actionId && p.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Permission>> GetByRoleAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.ActionId == _actionId && p.RoleId == roleId)
            .ToListAsync(cancellationToken);
    }

    public override async Task<IEnumerable<Permission>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(p => p.ActionId == _actionId).ToListAsync(cancellationToken);
    }

    public override async Task<Permission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await base.GetByIdAsync(id, cancellationToken);

        if (entity != null && entity.ActionId != _actionId)
            return null;

        return entity;
    }

    // Removed AddAsync as permissions should be managed via SetAsync (Upsert)
    // to maintain the unique constraint per User/Role per Action.
    public async Task<Permission> SetAsync(Permission permission, CancellationToken cancellationToken = default)
    {
        permission.ActionId = _actionId;

        var existing = await FindExistingAsync(permission, cancellationToken);

        if (existing != null)
        {
            existing.Outcome = permission.Outcome;
            return existing;
        }

        await base.AddAsync(permission, cancellationToken);
        return permission;
    }

    private async Task<Permission?> FindExistingAsync(Permission permission, CancellationToken cancellationToken)
    {
        return await _dbSet.FirstOrDefaultAsync(
            p => p.ActionId == _actionId && 
                 p.UserId == permission.UserId && 
                 p.RoleId == permission.RoleId, cancellationToken);
    }
}



