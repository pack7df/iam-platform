using IamPlatform.Domain.Identity;
using IamPlatform.Domain.Tenants;

namespace IamPlatform.Domain.Authorization;

public interface IAdminPrivilegeService
{
    Task<bool> HasGlobalAdminPrivilegesAsync(string userId, CancellationToken cancellationToken = default);
    Task<bool> HasTenantAdminPrivilegesAsync(string tenantId, string userId, CancellationToken cancellationToken = default);
}
