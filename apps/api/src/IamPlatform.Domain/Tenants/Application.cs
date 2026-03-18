using IamPlatform.Domain.Primitives;

namespace IamPlatform.Domain.Tenants;

public sealed class Application
{
    private Application(string id, string tenantId, string name, bool isActive)
    {
        Id = Guard.Required(id, nameof(id), "Application id is required.");
        TenantId = Guard.Required(tenantId, nameof(tenantId), "Tenant id is required.");
        Name = Guard.Required(name, nameof(name), "Application name is required.");
        IsActive = isActive;
    }

    public string Id { get; }

    public string TenantId { get; }

    public string Name { get; private set; }

    public bool IsActive { get; private set; }

    public static Application Create(string id, string tenantId, string name)
    {
        return new Application(id, tenantId, name, true);
    }

    public void Rename(string name)
    {
        Name = Guard.Required(name, nameof(name), "Application name is required.");
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
