using IamPlatform.Domain.Tenants;

namespace IamPlatform.Application.Identity;

public sealed record SystemUserBootstrapResult
{
    private SystemUserBootstrapResult(bool created, User? user, string? message = null)
    {
        IsCreated = created;
        User = user;
        Message = message;
    }

    public bool IsCreated { get; }

    public User? User { get; }

    public string? Message { get; }

    public bool IsSuccess => IsCreated || User != null;

    public static SystemUserBootstrapResult Created(User user)
    {
        return new SystemUserBootstrapResult(true, user);
    }

    public static SystemUserBootstrapResult AlreadyBootstrapped()
    {
        return new SystemUserBootstrapResult(false, null, "System user already exists.");
    }
}
