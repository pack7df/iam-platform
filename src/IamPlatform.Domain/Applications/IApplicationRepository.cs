using IamPlatform.Domain.Common;

namespace IamPlatform.Domain.Applications;

public interface IApplicationRepository : IRepository<Application>
{
    Task<Application?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    IResourceRepository GetResourceRepository(Guid applicationId);
}

