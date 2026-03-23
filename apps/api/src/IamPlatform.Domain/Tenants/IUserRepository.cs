namespace IamPlatform.Domain.Tenants;

public interface IUserRepository
{
    Task AddAsync(User user, CancellationToken cancellationToken = default);
}
