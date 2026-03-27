using IamPlatform.Domain.Authorization;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace IamPlatform.Application.Authorization.Rules;

public sealed record GetAuthorizationRuleQuery(string Id) : IRequest<AuthorizationRuleResponse?>;

public sealed class GetAuthorizationRuleHandler : IRequestHandler<GetAuthorizationRuleQuery, AuthorizationRuleResponse?>
{
    private readonly IAuthorizationRuleRepository _repository;

    public GetAuthorizationRuleHandler(IAuthorizationRuleRepository repository)
    {
        _repository = repository;
    }

    public async Task<AuthorizationRuleResponse?> Handle(GetAuthorizationRuleQuery request, CancellationToken cancellationToken)
    {
        var rule = await _repository.GetByIdAsync(request.Id, cancellationToken);
        
        if (rule == null) return null;

        return new AuthorizationRuleResponse(
            rule.Id,
            rule.UserId,
            rule.RoleId,
            rule.ResourceId,
            rule.OperationId,
            rule.Decision.ToString(),
            rule.IsActive);
    }
}
