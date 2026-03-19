namespace IamPlatform.Domain.Identity;

public interface ISystemUserRepository
{
    Task<bool> ExistsAnySystemUserAsync(CancellationToken cancellationToken = default);

    Task AddAsync(SystemUser systemUser, CancellationToken cancellationToken = default);
}
