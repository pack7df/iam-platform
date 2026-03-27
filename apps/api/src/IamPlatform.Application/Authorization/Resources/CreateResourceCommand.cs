using System;
using System.Threading;
using System.Threading.Tasks;
using IamPlatform.Domain.Authorization;
using IamPlatform.Domain.Common;

namespace IamPlatform.Application.Authorization.Resources;

public sealed class CreateResourceCommand
{
    public string Id { get; init; } = string.Empty;
    public string ApplicationId { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Key { get; init; } = string.Empty;
    public string? ParentId { get; init; }
    public string TenantId { get; init; } = string.Empty;
}

public interface ICreateResourceHandler
{
    Task HandleAsync(CreateResourceCommand command, CancellationToken cancellationToken = default);
}

public sealed class CreateResourceHandler : ICreateResourceHandler
{
    private readonly IResourceRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateResourceHandler(IResourceRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(CreateResourceCommand command, CancellationToken cancellationToken = default)
    {
        var existing = await _repository.GetByIdAsync(command.Id, cancellationToken);
        if (existing != null)
        {
            throw new InvalidOperationException($"Resource with ID {command.Id} already exists.");
        }

        IamPlatform.Domain.Authorization.Resource resource;
        if (string.IsNullOrEmpty(command.ParentId))
        {
            resource = IamPlatform.Domain.Authorization.Resource.CreateRoot(command.Id, command.ApplicationId, command.Name, command.Key);
        }
        else
        {
            var parent = await _repository.GetByIdAsync(command.ParentId, cancellationToken);
            if (parent == null)
            {
                throw new InvalidOperationException($"Parent resource with ID {command.ParentId} not found.");
            }
            resource = IamPlatform.Domain.Authorization.Resource.CreateChild(command.Id, command.ApplicationId, command.Name, command.Key, command.ParentId);
        }

        await _repository.AddAsync(resource, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
