using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IamPlatform.Domain.Authorization;

public interface IOperationRepository
{
    Task<Operation?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Operation>> GetAllForApplicationAsync(string applicationId, CancellationToken cancellationToken = default);
    Task AddAsync(Operation operation, CancellationToken cancellationToken = default);
    Task UpdateAsync(Operation operation, CancellationToken cancellationToken = default);
    Task RemoveAsync(Operation operation, CancellationToken cancellationToken = default);
}
