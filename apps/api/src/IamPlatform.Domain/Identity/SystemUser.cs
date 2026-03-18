using IamPlatform.Domain.Primitives;

namespace IamPlatform.Domain.Identity;

public sealed class SystemUser
{
    private SystemUser(string id, bool isActive)
    {
        Id = Guard.Required(id, nameof(id), "System user id is required.");
        IsActive = isActive;
    }

    public string Id { get; }

    public bool IsActive { get; private set; }

    public static SystemUser Create(string id)
    {
        return new SystemUser(id, true);
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
