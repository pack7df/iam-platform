namespace IamPlatform.Domain.Authorization;

public sealed record AuthorizationResult(
    bool IsAuthorized,
    AuthorizationRuleDecision Decision,
    string? ResolvedResourceId,
    IReadOnlyList<AuthorizationRule> AppliedRules)
{
    public static AuthorizationResult Authorized(string resolvedResourceId, IReadOnlyList<AuthorizationRule> appliedRules)
    {
        return new AuthorizationResult(true, AuthorizationRuleDecision.Allow, resolvedResourceId, appliedRules);
    }

    public static AuthorizationResult Denied(string? resolvedResourceId, IReadOnlyList<AuthorizationRule> appliedRules)
    {
        return new AuthorizationResult(false, AuthorizationRuleDecision.Deny, resolvedResourceId, appliedRules);
    }
}