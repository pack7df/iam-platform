using IamPlatform.Domain.Authorization;
using IamPlatform.Domain.Common;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace IamPlatform.Application.Authorization.Rules;

public sealed record UpdateAuthorizationRuleCommand(
    string Id,
    string Decision,
    bool IsActive) : IRequest;

public sealed class UpdateAuthorizationRuleHandler : IRequestHandler<UpdateAuthorizationRuleCommand>
{
    private readonly IAuthorizationRuleRepository _repository;
    private readonly IUnitOfWork _uow;

    public UpdateAuthorizationRuleHandler(IAuthorizationRuleRepository repository, IUnitOfWork uow)
    {
        _repository = repository;
        _uow = uow;
    }

    public async Task Handle(UpdateAuthorizationRuleCommand request, CancellationToken cancellationToken)
    {
        var rule = await _repository.GetByIdAsync(request.Id, cancellationToken);
        
        if (rule == null) return;

        if (Enum.TryParse<AuthorizationRuleDecision>(request.Decision, true, out var decision))
        {
            rule.ChangeDecision(decision);
        }

        if (request.IsActive) rule.Activate();
        else rule.Deactivate();

        await _repository.UpdateAsync(rule, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}
