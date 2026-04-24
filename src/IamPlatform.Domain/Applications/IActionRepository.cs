using IamPlatform.Domain.Common;

namespace IamPlatform.Domain.Applications;

public interface IActionRepository : IRepository<Action>
{
    Task<Action?> GetByOperationAsync(Guid operationId, CancellationToken cancellationToken = default);
}
