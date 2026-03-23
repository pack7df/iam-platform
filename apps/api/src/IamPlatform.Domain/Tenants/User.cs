using IamPlatform.Domain.Primitives;

namespace IamPlatform.Domain.Tenants;

public sealed class User
{
    private User(string id, string tenantId, UserType type, bool isActive)
    {
        Id = Guard.Required(id, nameof(id), "User id is required.");
        TenantId = Guard.Required(tenantId, nameof(tenantId), "Tenant id is required.");
        Type = type;
        IsActive = isActive;
    }

    public string Id { get; }

    public string TenantId { get; }

    public UserType Type { get; private set; }

    public bool IsActive { get; private set; }

    public static User Create(string id, string tenantId, UserType type)
    {
        return new User(id, tenantId, type, true);
    }

    public void ChangeType(UserType type)
    {
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
