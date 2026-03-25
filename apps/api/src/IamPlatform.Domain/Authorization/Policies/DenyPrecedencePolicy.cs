using IamPlatform.Domain.Primitives;

namespace IamPlatform.Domain.Authorization;

public sealed class DenyPrecedencePolicy : IAuthorizationPolicy
{
    public AuthorizationRuleDecision Aggregate(IReadOnlyList<AuthorizationRuleDecision> resolvedDecisions)
    {
        Guard.Required(resolvedDecisions, nameof(resolvedDecisions));

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