using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IamPlatform.Domain.Authorization;
using MediatR;

namespace IamPlatform.Application.Authorization.Resources;

public sealed record ListResourcesQuery(string ApplicationId) : IRequest<IReadOnlyCollection<ResourceResponse>>;

public sealed class ListResourcesHandler : IRequestHandler<ListResourcesQuery, IReadOnlyCollection<ResourceResponse>>
{
    private readonly IResourceRepository _repository;

    public ListResourcesHandler(IResourceRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<ResourceResponse>> Handle(ListResourcesQuery request, CancellationToken cancellationToken)
    {
        var resources = await _repository.GetAllForApplicationAsync(request.ApplicationId, cancellationToken);
        
        return resources.Select(r => new ResourceResponse(
            r.Id,
            r.Name,
            r.ParentId,
            r.ApplicationId,
            r.IsActive)).ToList();
    }
}
