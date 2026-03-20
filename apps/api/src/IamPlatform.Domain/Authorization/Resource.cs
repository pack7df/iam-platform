using IamPlatform.Domain.Primitives;

namespace IamPlatform.Domain.Authorization;

public sealed class Resource
{
    private Resource(string id, string applicationId, string name, string key, string? parentId, bool isActive)
    {
        Id = Guard.Required(id, nameof(id), "Resource id is required.");
        ApplicationId = Guard.Required(applicationId, nameof(applicationId), "Application id is required.");
        Name = Guard.Required(name, nameof(name), "Resource name is required.");
        Key = Guard.Required(key, nameof(key), "Resource key is required.");
        ParentId = string.IsNullOrWhiteSpace(parentId) ? null : parentId.Trim();
        IsActive = isActive;
    }

    public string Id { get; }

    public string ApplicationId { get; }

    public string Name { get; private set; }

    public string Key { get; private set; }

    public string? ParentId { get; }

    public bool IsActive { get; private set; }

    public bool IsRoot => ParentId is null;

    public static Resource CreateRoot(string id, string applicationId, string name, string key)
    {
        return new Resource(id, applicationId, name, key, null, true);
    }

    public static Resource CreateChild(string id, string applicationId, string name, string key, string parentId)
    {
        return new Resource(id, applicationId, name, key, Guard.Required(parentId, nameof(parentId), "Parent resource id is required."), true);
    }

    public void Rename(string name)
    {
        Name = Guard.Required(name, nameof(name), "Resource name is required.");
    }

    public void ChangeKey(string key)
    {
        Key = Guard.Required(key, nameof(key), "Resource key is required.");
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
