namespace IamPlatform.Domain.Authorization;

public sealed class DenyPrecedencePolicy
{
    public AuthorizationRuleDecision Aggregate(IReadOnlyList<AuthorizationRuleDecision> resolvedDecisions)
    {
        ArgumentNullException.ThrowIfNull(resolvedDecisions);

        if (resolvedDecisions.Count == 0)
        {
            return AuthorizationRuleDecision.Deny;
        }

        if (resolvedDecisions.Contains(AuthorizationRuleDecision.Deny))
        {
            return AuthorizationRuleDecision.Deny;
        }

        if (resolvedDecisions.All(d => d == AuthorizationRuleDecision.Allow))
        {
            return AuthorizationRuleDecision.Allow;
        }

        return AuthorizationRuleDecision.Deny;
    }
}