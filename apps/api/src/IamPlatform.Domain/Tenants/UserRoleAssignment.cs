using IamPlatform.Domain.Primitives;

namespace IamPlatform.Domain.Tenants;

public sealed class UserRoleAssignment
{
    private UserRoleAssignment(string id, string tenantUserId, string roleId, bool isActive)
    {
        Id = Guard.Required(id, nameof(id), "Tenant user role assignment id is required.");
        UserId = Guard.Required(tenantUserId, nameof(tenantUserId), "Tenant user id is required.");
        RoleId = Guard.Required(roleId, nameof(roleId), "Role id is required.");
        IsActive = isActive;
    }

    public string Id { get; }

    public string UserId { get; }

    public string RoleId { get; }

    public bool IsActive { get; private set; }

    public static UserRoleAssignment Assign(string id, User tenantUser, Role role)
    {
        ArgumentNullException.ThrowIfNull(tenantUser);
        ArgumentNullException.ThrowIfNull(role);

        if (tenantUser.TenantId != role.TenantId)
        {
            throw new InvalidOperationException("Tenant user and role must belong to the same tenant.");
        }

        return new UserRoleAssignment(id, tenantUser.Id, role.Id, true);
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
