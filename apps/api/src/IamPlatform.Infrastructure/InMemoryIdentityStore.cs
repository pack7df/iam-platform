using IamPlatform.Domain.Identity;

namespace IamPlatform.Infrastructure;

internal sealed class InMemoryIdentityStore
{
    public List<SystemUser> SystemUsers { get; } = new();

    public List<Invitation> Invitations { get; } = new();
}
