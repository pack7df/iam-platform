using IamPlatform.Domain.Tenants;

namespace IamPlatform.Application.Identity;

public sealed record SystemUserBootstrapResult
{
    private SystemUserBootstrapResult(bool created, User? systemUser)
    {
        IsCreated = created;
        SystemUser = systemUser;
    }

    public bool IsCreated { get; }

    public User? SystemUser { get; }

    public static SystemUserBootstrapResult Created(User systemUser)
    {
        return new SystemUserBootstrapResult(true, systemUser);
    }

    public static SystemUserBootstrapResult AlreadyBootstrapped()
    {
        return new SystemUserBootstrapResult(false, null);
    }
}
