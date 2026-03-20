namespace IamPlatform.Domain.Authorization;

public static class AuthorizationRuleDecisionExtensions
{
    public static bool IsExplicit(this AuthorizationRuleDecision decision)
    {
        return decision is AuthorizationRuleDecision.Allow or AuthorizationRuleDecision.Deny;
    }

    public static bool IsInherited(this AuthorizationRuleDecision decision)
    {
        return decision == AuthorizationRuleDecision.Inherit;
    }

    public static bool RequiresParentResolution(this AuthorizationRuleDecision decision)
    {
        return decision.IsInherited();
    }

    public static bool IsResolved(this AuthorizationRuleDecision decision)
    {
        return decision.IsExplicit();
    }
}
