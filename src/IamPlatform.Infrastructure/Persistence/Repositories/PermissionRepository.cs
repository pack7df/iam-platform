using IamPlatform.Domain.Authorization;
using Microsoft.EntityFrameworkCore;

namespace IamPlatform.Infrastructure.Persistence.Repositories;

public class PermissionRepository : BaseRepository<Permission>, IPermissionRepository
{
    private readonly Guid? _actionId;

    // Internal constructor for DI if needed globally (e.g. for querying by User/Role across actions)
    public PermissionRepository(IamPlatformDbContext context) : base(context)
    {
    }

    // Constructor used by ActionRepository builder
    public PermissionRepository(IamPlatformDbContext context, Guid actionId) : base(context)
    {
        _actionId = actionId;
    }

    public async Task<IEnumerable<Permission>> GetByActionAsync(Guid actionId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(p => p.ActionId == actionId).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Permission>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(p => p.UserId == userId).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Permission>> GetByRoleAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(p => p.RoleId == roleId).ToListAsync(cancellationToken);
    }

    public override async Task<IEnumerable<Permission>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        if (_actionId.HasValue)
        {
            return await GetByActionAsync(_actionId.Value, cancellationToken);
        }
        return await base.GetAllAsync(cancellationToken);
    }

    public override async Task AddAsync(Permission entity, CancellationToken cancellationToken = default)
    {
        if (_actionId.HasValue)
        {
            entity.ActionId = _actionId.Value;
        }
        await base.AddAsync(entity, cancellationToken);
    }
}
