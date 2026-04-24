using IamPlatform.Domain.Common;

namespace IamPlatform.Domain.Tenants;

public interface IRoleRepository : IRepository<Role>
{
    Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}
