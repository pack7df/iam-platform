using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IamPlatform.Domain.Authorization;

namespace IamPlatform.Application.Authorization.Resources;

public interface IListResourcesHandler
{
    Task<IReadOnlyCollection<ResourceResponse>> HandleAsync(string applicationId, CancellationToken cancellationToken = default);
}

public sealed class ListResourcesHandler : IListResourcesHandler
{
    private readonly IResourceRepository _repository;

    public ListResourcesHandler(IResourceRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<ResourceResponse>> HandleAsync(string applicationId, CancellationToken cancellationToken = default)
    {
        var resources = await _repository.GetAllForApplicationAsync(applicationId, cancellationToken);
        
        return resources.Select(r => new ResourceResponse(
            r.Id,
            r.Name,
            r.ParentId,
            r.ApplicationId,
            r.IsActive)).ToList();
    }
}
