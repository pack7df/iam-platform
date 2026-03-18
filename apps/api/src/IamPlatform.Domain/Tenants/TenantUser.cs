using IamPlatform.Domain.Primitives;

namespace IamPlatform.Domain.Tenants;

public sealed class TenantUser
{
    private TenantUser(string id, string tenantId, TenantUserType type, bool isActive)
    {
        Id = Guard.Required(id, nameof(id), "Tenant user id is required.");
        TenantId = Guard.Required(tenantId, nameof(tenantId), "Tenant id is required.");
        Type = type;
        IsActive = isActive;
    }

    public string Id { get; }

    public string TenantId { get; }

    public TenantUserType Type { get; private set; }

    public bool IsActive { get; private set; }

    public static TenantUser Create(string id, string tenantId, TenantUserType type)
    {
        return new TenantUser(id, tenantId, type, true);
    }

    public void ChangeType(TenantUserType type)
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
