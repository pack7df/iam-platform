using MediatR;

namespace IamPlatform.Application.Authorization.Evaluation;

public record EvaluateAuthorizationQuery(string UserId, string ResourceId, string OperationId) 
    : IRequest<EvaluationResponse>;

public record EvaluationResponse(
    bool IsAuthorized, 
    string Decision, 
    string? ResolvedResourceId, 
    List<string> AppliedRuleIds);
