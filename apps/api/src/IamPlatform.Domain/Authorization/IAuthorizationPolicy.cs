namespace IamPlatform.Domain.Authorization;

public interface IAuthorizationPolicy
{
    AuthorizationRuleDecision Aggregate(IReadOnlyList<AuthorizationRuleDecision> resolvedDecisions);
}