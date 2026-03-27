using IamPlatform.Domain.Authorization;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IamPlatform.Application.Authorization.Rules;

public sealed record ListAuthorizationRulesQuery(
    string? ResourceId = null,
    string? OperationId = null) : IRequest<IReadOnlyCollection<AuthorizationRuleResponse>>;

public sealed class ListAuthorizationRulesHandler : IRequestHandler<ListAuthorizationRulesQuery, IReadOnlyCollection<AuthorizationRuleResponse>>
{
    private readonly IAuthorizationRuleRepository _repository;

    public ListAuthorizationRulesHandler(IAuthorizationRuleRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<AuthorizationRuleResponse>> Handle(ListAuthorizationRulesQuery request, CancellationToken cancellationToken)
    {
        // For simplicity in this task, we'll get all and filter in memory if needed, 
        // but ideally the repository should support this.
        // Let's check IAuthorizationRuleRepository first.
        var rules = await _repository.GetAllAsync(cancellationToken);
        
        var query = rules.AsQueryable();

        if (!string.IsNullOrEmpty(request.ResourceId))
        {
            query = query.Where(r => r.ResourceId == request.ResourceId);
        }

        if (!string.IsNullOrEmpty(request.OperationId))
        {
            query = query.Where(r => r.OperationId == request.OperationId);
        }

        return query.Select(rule => new AuthorizationRuleResponse(
            rule.Id,
            rule.UserId,
            rule.RoleId,
            rule.ResourceId,
            rule.OperationId,
            rule.Decision.ToString(),
            rule.IsActive)).ToList();
    }
}
