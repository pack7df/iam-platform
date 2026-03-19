using IamPlatform.Domain.Primitives;

namespace IamPlatform.Domain.Tenants;

public sealed class Role
{
    private Role(string id, string tenantId, string name, bool isActive)
    {
        Id = Guard.Required(id, nameof(id), "Role id is required.");
        TenantId = Guard.Required(tenantId, nameof(tenantId), "Tenant id is required.");
        Name = Guard.Required(name, nameof(name), "Role name is required.");
        IsActive = isActive;
    }

    public string Id { get; }

    public string TenantId { get; }

    public string Name { get; private set; }

    public bool IsActive { get; private set; }

    public static Role Create(string id, string tenantId, string name)
    {
        return new Role(id, tenantId, name, true);
    }

    public void Rename(string name)
    {
        Name = Guard.Required(name, nameof(name), "Role name is required.");
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
