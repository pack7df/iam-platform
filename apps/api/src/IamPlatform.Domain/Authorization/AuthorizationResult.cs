namespace IamPlatform.Domain.Authorization;

public sealed record AuthorizationResult(
    bool IsAuthorized,
    AuthorizationRuleDecision Decision,
    string? ResolvedResourceId,
    string OperationId,
    IReadOnlyList<AuthorizationRule> AppliedRules)
{
    public static AuthorizationResult Authorized(string? resolvedResourceId, string operationId, IReadOnlyList<AuthorizationRule> appliedRules)
    {
        return new AuthorizationResult(true, AuthorizationRuleDecision.Allow, resolvedResourceId, operationId, appliedRules);
    }

    public static AuthorizationResult Denied(string? resolvedResourceId, string operationId, IReadOnlyList<AuthorizationRule> appliedRules)
    {
        return new AuthorizationResult(false, AuthorizationRuleDecision.Deny, resolvedResourceId, operationId, appliedRules);
    }
}