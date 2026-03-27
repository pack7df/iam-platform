using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IamPlatform.Domain.Tenants;

public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Role>> GetByTenantIdAsync(string tenantId, CancellationToken cancellationToken = default);
    Task AddAsync(Role role, CancellationToken cancellationToken = default);
    Task UpdateAsync(Role role, CancellationToken cancellationToken = default);
}
