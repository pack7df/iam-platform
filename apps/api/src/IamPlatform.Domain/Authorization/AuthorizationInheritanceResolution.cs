namespace IamPlatform.Domain.Authorization;

public sealed record AuthorizationInheritanceResolution(
    bool IsResolved,
    AuthorizationRuleDecision? Decision,
    string? ResolvedResourceId)
{
    public static AuthorizationInheritanceResolution Resolved(AuthorizationRuleDecision decision, string resourceId)
    {
        return new AuthorizationInheritanceResolution(true, decision, resourceId);
    }

    public static AuthorizationInheritanceResolution Unresolved()
    {
        return new AuthorizationInheritanceResolution(false, null, null);
    }
}
