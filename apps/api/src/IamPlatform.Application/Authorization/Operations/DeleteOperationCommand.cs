using IamPlatform.Domain.Authorization;
using IamPlatform.Domain.Common;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace IamPlatform.Application.Authorization.Operations;

public sealed record DeleteOperationCommand(string Id) : IRequest;

public sealed class DeleteOperationHandler : IRequestHandler<DeleteOperationCommand>
{
    private readonly IOperationRepository _repository;
    private readonly IUnitOfWork _uow;

    public DeleteOperationHandler(IOperationRepository repository, IUnitOfWork uow)
    {
        _repository = repository;
        _uow = uow;
    }

    public async Task Handle(DeleteOperationCommand request, CancellationToken cancellationToken)
    {
        var operation = await _repository.GetByIdAsync(request.Id, cancellationToken);
        
        if (operation == null) return;

        await _repository.RemoveAsync(operation, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}
