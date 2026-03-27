using IamPlatform.Domain.Authorization;
using IamPlatform.Domain.Common;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace IamPlatform.Application.Authorization.Rules;

public sealed record DeleteAuthorizationRuleCommand(string Id) : IRequest;

public sealed class DeleteAuthorizationRuleHandler : IRequestHandler<DeleteAuthorizationRuleCommand>
{
    private readonly IAuthorizationRuleRepository _repository;
    private readonly IUnitOfWork _uow;

    public DeleteAuthorizationRuleHandler(IAuthorizationRuleRepository repository, IUnitOfWork uow)
    {
        _repository = repository;
        _uow = uow;
    }

    public async Task Handle(DeleteAuthorizationRuleCommand request, CancellationToken cancellationToken)
    {
        var rule = await _repository.GetByIdAsync(request.Id, cancellationToken);
        
        if (rule == null) return;

        await _repository.RemoveAsync(rule, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}
