namespace IamPlatform.Domain.Identity;

public interface ISystemUserRepository
{
    Task<SystemUser?> GetByIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAnySystemUserAsync(CancellationToken cancellationToken = default);
    Task AddAsync(SystemUser systemUser, CancellationToken cancellationToken = default);
}
