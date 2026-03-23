using IamPlatform.Domain.Identity;
using IamPlatform.Domain.Primitives;
using IamPlatform.Domain.Tenants;

namespace IamPlatform.Domain.Authorization;

public sealed class ImplicitAdministrativePrivilegePolicy
{
    public bool CanManagePlatform(SystemUser systemUser)
    {
        ArgumentNullException.ThrowIfNull(systemUser);

        return systemUser.IsActive;
    }

    public bool CanManageTenant(User tenantUser, string tenantId)
    {
        ArgumentNullException.ThrowIfNull(tenantUser);

        var requiredTenantId = Guard.Required(tenantId, nameof(tenantId), "Tenant id is required.");

        return tenantUser.IsActive
            && tenantUser.Type == UserType.TenantAdmin
            && tenantUser.TenantId == requiredTenantId;
    }
}
