namespace IamPlatform.Domain.Identity;

public interface IInvitationRepository
{
    Task AddAsync(Invitation invitation, CancellationToken cancellationToken = default);
}
