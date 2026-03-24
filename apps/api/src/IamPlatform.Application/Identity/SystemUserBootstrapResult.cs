using IamPlatform.Domain.Identity;

namespace IamPlatform.Application.Identity;

public sealed record SystemUserBootstrapResult
{
    private SystemUserBootstrapResult(bool created, SystemUser? systemUser)
    {
        IsCreated = created;
        SystemUser = systemUser;
    }

    public bool IsCreated { get; }

    public SystemUser? SystemUser { get; }

    public static SystemUserBootstrapResult Created(SystemUser systemUser)
    {
        return new SystemUserBootstrapResult(true, systemUser);
    }

    public static SystemUserBootstrapResult AlreadyBootstrapped()
    {
        return new SystemUserBootstrapResult(false, null);
    }
}
