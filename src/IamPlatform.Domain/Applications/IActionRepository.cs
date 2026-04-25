using IamPlatform.Domain.Common;
using IamPlatform.Domain.Authorization;

namespace IamPlatform.Domain.Applications;

public interface IActionRepository : IRepository<Action>
{
    Task<Action?> GetByOperationAsync(Guid operationId, CancellationToken cancellationToken = default);
    IPermissionRepository GetPermissionRepository(Guid actionId);
}

