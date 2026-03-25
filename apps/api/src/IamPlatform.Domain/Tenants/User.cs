using IamPlatform.Domain.Primitives;

namespace IamPlatform.Domain.Tenants;

public sealed class User
{
    private User(string id, string? tenantId, UserType type, bool isActive)
    {
        Id = Guard.Required(id, nameof(id), "User id is required.");
        
        // Validation: SystemUser can have null tenantId, but TenantUser must have tenantId
        if (type != UserType.SystemUser)
        {
            TenantId = Guard.Required(tenantId, nameof(tenantId), "Tenant id is required for non-system users.");
        }
        else
        {
            // SystemUser: tenantId must be null or empty (normalize to null)
            TenantId = string.IsNullOrWhiteSpace(tenantId) ? null : tenantId.Trim();
        }
        
        Type = type;
        IsActive = isActive;
    }

    public string Id { get; }

    public string? TenantId { get; }

    public UserType Type { get; private set; }

    public bool IsActive { get; private set; }

    public static User Create(string id, string tenantId, UserType type)
    {
        return new User(id, tenantId, type, true);
    }

    public static User CreateSystemUser(string id)
    {
        return new User(id, null, UserType.SystemUser, true);
    }

    public void ChangeType(UserType type)
    {
        // If changing to non-SystemUser, ensure TenantId is set
        if (type != UserType.SystemUser && string.IsNullOrWhiteSpace(TenantId))
        {
            throw new InvalidOperationException("Cannot change to non-system user without a tenant id.");
        }
        Type = type;
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
