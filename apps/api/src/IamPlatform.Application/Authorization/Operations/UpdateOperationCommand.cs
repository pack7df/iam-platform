using IamPlatform.Domain.Authorization;
using IamPlatform.Domain.Common;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace IamPlatform.Application.Authorization.Operations;

public sealed record UpdateOperationCommand(
    string Id,
    string Name,
    string Key,
    bool IsActive) : IRequest;

public sealed class UpdateOperationHandler : IRequestHandler<UpdateOperationCommand>
{
    private readonly IOperationRepository _repository;
    private readonly IUnitOfWork _uow;

    public UpdateOperationHandler(IOperationRepository repository, IUnitOfWork uow)
    {
        _repository = repository;
        _uow = uow;
    }

    public async Task Handle(UpdateOperationCommand request, CancellationToken cancellationToken)
    {
        var operation = await _repository.GetByIdAsync(request.Id, cancellationToken);
        
        if (operation == null) return;

        operation.Rename(request.Name);
        operation.ChangeKey(request.Key);

        if (request.IsActive) operation.Activate();
        else operation.Deactivate();

        await _repository.UpdateAsync(operation, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}
