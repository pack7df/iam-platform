using IamPlatform.Domain.Authorization;

namespace IamPlatform.Domain.Authorization;

public interface IResourceRepository
{
    Task<IReadOnlyCollection<Resource>> GetAllForApplicationAsync(string applicationId, CancellationToken cancellationToken = default);
    Task<Resource?> GetByIdAsync(string resourceId, CancellationToken cancellationToken = default);
}