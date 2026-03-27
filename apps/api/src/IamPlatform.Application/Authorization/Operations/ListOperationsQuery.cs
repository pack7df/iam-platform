using IamPlatform.Domain.Authorization;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IamPlatform.Application.Authorization.Operations;

public sealed record ListOperationsQuery(string ApplicationId) : IRequest<IReadOnlyCollection<OperationResponse>>;

public sealed class ListOperationsHandler : IRequestHandler<ListOperationsQuery, IReadOnlyCollection<OperationResponse>>
{
    private readonly IOperationRepository _repository;

    public ListOperationsHandler(IOperationRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<OperationResponse>> Handle(ListOperationsQuery request, CancellationToken cancellationToken)
    {
        var operations = await _repository.GetAllForApplicationAsync(request.ApplicationId, cancellationToken);
        
        return operations.Select(o => new OperationResponse(
            o.Id,
            o.Name,
            o.Key,
            o.ApplicationId,
            o.IsActive)).ToList();
    }
}
