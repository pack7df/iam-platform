namespace IamPlatform.Domain.Tenants;

public sealed class Tenant
{
    private Tenant(string id, string name, bool isActive)
    {
        Id = GuardRequired(id, nameof(id), "Tenant id is required.");
        Name = GuardRequired(name, nameof(name), "Tenant name is required.");
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
        Name = GuardRequired(name, nameof(name), "Tenant name is required.");
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    private static string GuardRequired(string value, string paramName, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(message, paramName);
        }

        return value.Trim();
    }
}
