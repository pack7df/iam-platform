using IamPlatform.Domain.Common;

namespace IamPlatform.Domain.Authorization;

public interface IPermissionRepository : IRepository<Permission>
{
    Task<IEnumerable<Permission>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Permission>> GetByRoleAsync(Guid roleId, CancellationToken cancellationToken = default);
}

