using IamPlatform.Domain.Identity;
using IamPlatform.Domain.Tenants;

namespace IamPlatform.Infrastructure;

internal sealed class InMemoryIdentityStore
{
    // Store all users, including SystemUser (tenant_id = null, type = SystemUser)
    public List<User> Users { get; } = new();

    public List<Invitation> Invitations { get; } = new();
}
