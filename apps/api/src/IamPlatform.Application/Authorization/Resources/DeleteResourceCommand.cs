using System.Threading;
using System.Threading.Tasks;
using IamPlatform.Domain.Authorization;
using IamPlatform.Domain.Common;
using MediatR;

namespace IamPlatform.Application.Authorization.Resources;

public sealed record DeleteResourceCommand(string Id) : IRequest;

public sealed class DeleteResourceHandler : IRequestHandler<DeleteResourceCommand>
{
    private readonly IResourceRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteResourceHandler(IResourceRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteResourceCommand request, CancellationToken cancellationToken)
    {
        var resource = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (resource == null)
        {
            return;
        }

        await _repository.RemoveAsync(resource, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
