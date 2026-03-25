namespace IamPlatform.Domain.Authorization;

public sealed record AuthorizationResult(
    string UserId,
    bool IsAuthorized,
    AuthorizationRuleDecision Decision,
    string? ResolvedResourceId,
    string OperationId,
    IReadOnlyList<AuthorizationRule> AppliedRules)
{
    public static AuthorizationResult Authorized(string userId, string? resolvedResourceId, string operationId, IReadOnlyList<AuthorizationRule> appliedRules)
    {
        return new AuthorizationResult(userId, true, AuthorizationRuleDecision.Allow, resolvedResourceId, operationId, appliedRules);
    }

    public static AuthorizationResult Denied(string userId, string? resolvedResourceId, string operationId, IReadOnlyList<AuthorizationRule> appliedRules)
    {
        return new AuthorizationResult(userId, false, AuthorizationRuleDecision.Deny, resolvedResourceId, operationId, appliedRules);
    }
}