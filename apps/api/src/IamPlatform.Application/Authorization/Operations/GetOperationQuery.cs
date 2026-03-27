using IamPlatform.Domain.Authorization;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace IamPlatform.Application.Authorization.Operations;

public sealed record GetOperationQuery(string Id) : IRequest<OperationResponse?>;

public sealed class GetOperationHandler : IRequestHandler<GetOperationQuery, OperationResponse?>
{
    private readonly IOperationRepository _repository;

    public GetOperationHandler(IOperationRepository repository)
    {
        _repository = repository;
    }

    public async Task<OperationResponse?> Handle(GetOperationQuery request, CancellationToken cancellationToken)
    {
        var operation = await _repository.GetByIdAsync(request.Id, cancellationToken);
        
        if (operation == null) return null;

        return new OperationResponse(
            operation.Id,
            operation.Name,
            operation.Key,
            operation.ApplicationId,
            operation.IsActive);
    }
}
