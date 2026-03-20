using IamPlatform.Domain.Primitives;

namespace IamPlatform.Domain.Authorization;

public sealed class Operation
{
    private Operation(string id, string applicationId, string key, string name, bool isActive)
    {
        Id = Guard.Required(id, nameof(id), "Operation id is required.");
        ApplicationId = Guard.Required(applicationId, nameof(applicationId), "Application id is required.");
        Key = Guard.Required(key, nameof(key), "Operation key is required.");
        Name = Guard.Required(name, nameof(name), "Operation name is required.");
        IsActive = isActive;
    }

    public string Id { get; }

    public string ApplicationId { get; }

    public string Key { get; private set; }

    public string Name { get; private set; }

    public bool IsActive { get; private set; }

    public static Operation Create(string id, string applicationId, string key, string name)
    {
        return new Operation(id, applicationId, key, name, true);
    }

    public void Rename(string name)
    {
        Name = Guard.Required(name, nameof(name), "Operation name is required.");
    }

    public void ChangeKey(string key)
    {
        Key = Guard.Required(key, nameof(key), "Operation key is required.");
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
