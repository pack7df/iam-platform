using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IamPlatform.Domain.Tenants;

public interface IApplicationRepository
{
    Task<IamPlatform.Domain.Tenants.Application?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<IamPlatform.Domain.Tenants.Application>> GetByTenantIdAsync(string tenantId, CancellationToken cancellationToken = default);
    Task AddAsync(IamPlatform.Domain.Tenants.Application application, CancellationToken cancellationToken = default);
    Task UpdateAsync(IamPlatform.Domain.Tenants.Application application, CancellationToken cancellationToken = default);
}
