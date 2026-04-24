using IamPlatform.Domain.Common;

namespace IamPlatform.Domain.Applications;

public interface IResourceRepository : IRepository<Resource>
{
    Task<Resource?> GetByKeyAsync(string key, CancellationToken cancellationToken = default);
    IActionRepository GetActionRepository(Guid resourceId);
}


