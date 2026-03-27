using System;
using System.Threading;
using System.Threading.Tasks;
using IamPlatform.Domain.Authorization;

namespace IamPlatform.Application.Authorization.Resources;

public sealed record ResourceResponse(
    string Id,
    string Name,
    string? ParentId,
    string ApplicationId,
    bool IsActive);

public interface IGetResourceHandler
{
    Task<ResourceResponse?> HandleAsync(string id, CancellationToken cancellationToken = default);
}

public sealed class GetResourceHandler : IGetResourceHandler
{
    private readonly IResourceRepository _repository;

    public GetResourceHandler(IResourceRepository repository)
    {
        _repository = repository;
    }

    public async Task<ResourceResponse?> HandleAsync(string id, CancellationToken cancellationToken = default)
    {
        var resource = await _repository.GetByIdAsync(id, cancellationToken);
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
