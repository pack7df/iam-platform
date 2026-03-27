using IamPlatform.Domain.Authorization;
using IamPlatform.Domain.Common;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace IamPlatform.Application.Authorization.Operations;

public sealed record CreateOperationCommand(
    string Id,
    string Name,
    string Key,
    string ApplicationId) : IRequest;

public sealed class CreateOperationHandler : IRequestHandler<CreateOperationCommand>
{
    private readonly IOperationRepository _repository;
    private readonly IUnitOfWork _uow;

    public CreateOperationHandler(IOperationRepository repository, IUnitOfWork uow)
    {
        _repository = repository;
        _uow = uow;
    }

    public async Task Handle(CreateOperationCommand request, CancellationToken cancellationToken)
    {
        var operation = Operation.Create(
            request.Id,
            request.ApplicationId,
            request.Key,
            request.Name);

        await _repository.AddAsync(operation, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}
