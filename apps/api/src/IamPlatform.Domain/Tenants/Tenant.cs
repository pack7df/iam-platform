using IamPlatform.Domain.Primitives;

namespace IamPlatform.Domain.Tenants;

public sealed class Tenant
{
    private Tenant(string id, string name, bool isActive)
    {
        Id = Guard.Required(id, nameof(id), "Tenant id is required.");
        Name = Guard.Required(name, nameof(name), "Tenant name is required.");
        IsActive = isActive;
    }

    public string Id { get; }

    public string Name { get; private set; }

    public bool IsActive { get; private set; }

    public static Tenant Create(string id, string name)
    {
        return new Tenant(id, name, true);
    }

    public void Rename(string name)
    {
        Name = Guard.Required(name, nameof(name), "Tenant name is required.");
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
