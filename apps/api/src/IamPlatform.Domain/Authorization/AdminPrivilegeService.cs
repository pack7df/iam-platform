using IamPlatform.Domain.Identity;
using IamPlatform.Domain.Tenants;

namespace IamPlatform.Domain.Authorization;

public sealed class AdminPrivilegeService : IAdminPrivilegeService
{
    private readonly ISystemUserRepository _systemUserRepository;
    private readonly IUserRepository _tenantUserRepository;

    public AdminPrivilegeService(
        ISystemUserRepository systemUserRepository,
        IUserRepository tenantUserRepository)
    {
        _systemUserRepository = systemUserRepository;
        _tenantUserRepository = tenantUserRepository;
    }

    public async Task<bool> HasGlobalAdminPrivilegesAsync(string userId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(userId);
        var systemUser = await _systemUserRepository.GetByIdAsync(userId, cancellationToken);
        return systemUser is not null;
    }

    public async Task<bool> HasTenantAdminPrivilegesAsync(string tenantId, string userId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(tenantId);
        ArgumentException.ThrowIfNullOrEmpty(userId);
        var tenantUser = await _tenantUserRepository.GetByIdAsync(userId, cancellationToken);
        return tenantUser is not null
               && tenantUser.TenantId == tenantId
               && tenantUser.Type == UserType.TenantAdmin;
    }
}
