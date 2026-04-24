using IamPlatform.Domain.Common;

namespace IamPlatform.Domain.Operations;

public interface IOperationRepository : IRepository<Operation>
{
    Task<Operation?> GetByKeyAsync(string key, CancellationToken cancellationToken = default);
}
