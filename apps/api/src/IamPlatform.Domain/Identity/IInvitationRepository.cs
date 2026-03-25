namespace IamPlatform.Domain.Identity;

public interface IInvitationRepository
{
    Task<Invitation?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task AddAsync(Invitation invitation, CancellationToken cancellationToken = default);
    Task UpdateAsync(Invitation invitation, CancellationToken cancellationToken = default);
}
