using IamPlatform.Domain.Primitives;
using IamPlatform.Domain.Tenants;
using System.Threading;
using System.Threading.Tasks;

namespace IamPlatform.Domain.Authorization;

public sealed class AdminPrivilegeService : IAdminPrivilegeService
{
    private readonly IUserRepository _userRepository;

    public AdminPrivilegeService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> HasGlobalAdminPrivilegesAsync(string userId, CancellationToken cancellationToken = default)
    {
        Guard.Required(userId, nameof(userId));
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        return user?.Type == UserType.SystemUser;
    }

    public async Task<bool> HasTenantAdminPrivilegesAsync(string tenantId, string userId, CancellationToken cancellationToken = default)
    {
        Guard.Required(tenantId, nameof(tenantId));
        Guard.Required(userId, nameof(userId));
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return false;
        }

        if (user.Type == UserType.SystemUser)
        {
            // System users have global privileges, so they have admin privileges in any tenant
            return true;
        }

        if (user.Type != UserType.TenantAdmin)
        {
            return false;
        }

        return user.TenantId == tenantId;
    }
}
