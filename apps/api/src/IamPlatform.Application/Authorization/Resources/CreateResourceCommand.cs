using System;
using System.Threading;
using System.Threading.Tasks;
using IamPlatform.Domain.Authorization;
using IamPlatform.Domain.Common;
using MediatR;

namespace IamPlatform.Application.Authorization.Resources;

public sealed record CreateResourceCommand(
    string Id,
    string ApplicationId,
    string Name,
    string Key,
    string? ParentId) : IRequest;

public sealed class CreateResourceHandler : IRequestHandler<CreateResourceCommand>
{
    private readonly IResourceRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateResourceHandler(IResourceRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(CreateResourceCommand request, CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (existing != null)
        {
            throw new InvalidOperationException($"Resource with ID {request.Id} already exists.");
        }

        Resource resource;
        if (string.IsNullOrEmpty(request.ParentId))
        {
            resource = Resource.CreateRoot(request.Id, request.ApplicationId, request.Name, request.Key);
        }
        else
        {
            var parent = await _repository.GetByIdAsync(request.ParentId, cancellationToken);
            if (parent == null)
            {
                throw new InvalidOperationException($"Parent resource with ID {request.ParentId} not found.");
            }
            resource = Resource.CreateChild(request.Id, request.ApplicationId, request.Name, request.Key, request.ParentId);
        }

        await _repository.AddAsync(resource, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
