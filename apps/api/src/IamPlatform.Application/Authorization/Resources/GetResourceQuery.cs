using System;
using System.Threading;
using System.Threading.Tasks;
using IamPlatform.Domain.Authorization;
using MediatR;

namespace IamPlatform.Application.Authorization.Resources;

public sealed record GetResourceQuery(string Id) : IRequest<ResourceResponse?>;

public sealed class GetResourceHandler : IRequestHandler<GetResourceQuery, ResourceResponse?>
{
    private readonly IResourceRepository _repository;

    public GetResourceHandler(IResourceRepository repository)
    {
        _repository = repository;
    }

    public async Task<ResourceResponse?> Handle(GetResourceQuery request, CancellationToken cancellationToken)
    {
        var resource = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (resource == null)
        {
            return null;
        }

        return new ResourceResponse(
            resource.Id,
            resource.Name,
            resource.ParentId,
            resource.ApplicationId,
            resource.IsActive);
    }
}
