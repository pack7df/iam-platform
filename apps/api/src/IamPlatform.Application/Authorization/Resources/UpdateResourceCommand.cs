using System;
using System.Threading;
using System.Threading.Tasks;
using IamPlatform.Domain.Authorization;
using IamPlatform.Domain.Common;
using MediatR;

namespace IamPlatform.Application.Authorization.Resources;

public sealed record UpdateResourceCommand(
    string Id,
    string Name) : IRequest;

public sealed class UpdateResourceHandler : IRequestHandler<UpdateResourceCommand>
{
    private readonly IResourceRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateResourceHandler(IResourceRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateResourceCommand request, CancellationToken cancellationToken)
    {
        var resource = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (resource == null)
        {
            throw new InvalidOperationException($"Resource with ID {request.Id} not found.");
        }

        resource.Rename(request.Name);
        await _repository.UpdateAsync(resource, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
