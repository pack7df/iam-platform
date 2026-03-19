using IamPlatform.Domain.Identity;

namespace IamPlatform.Infrastructure;

internal sealed class InMemoryInvitationRepository : IInvitationRepository
{
    private readonly InMemoryIdentityStore _store;

    public InMemoryInvitationRepository(InMemoryIdentityStore store)
    {
        _store = store;
    }

    public Task AddAsync(Invitation invitation, CancellationToken cancellationToken = default)
    {
        _store.Invitations.Add(invitation);
        return Task.CompletedTask;
    }
}
