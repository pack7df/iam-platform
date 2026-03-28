using IamPlatform.Domain.Authorization;
using MediatR;

namespace IamPlatform.Application.Authorization.Evaluation;

public sealed class EvaluateAuthorizationHandler : IRequestHandler<EvaluateAuthorizationQuery, EvaluationResponse>
{
    private readonly IAuthorizationEngine _authorizationEngine;

    public EvaluateAuthorizationHandler(IAuthorizationEngine authorizationEngine)
    {
        _authorizationEngine = authorizationEngine;
    }

    public async Task<EvaluationResponse> Handle(EvaluateAuthorizationQuery request, CancellationToken cancellationToken)
    {
        var result = await _authorizationEngine.EvaluateAsync(
            request.UserId,
            request.ResourceId,
            request.OperationId,
            cancellationToken);

        return new EvaluationResponse(
            result.IsAuthorized,
            result.Decision.ToString(),
            result.ResolvedResourceId,
            result.AppliedRules.Select(r => r.Id).ToList());
    }
}
