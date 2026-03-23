using IamPlatform.Domain.Authorization;

namespace IamPlatform.Domain.Authorization;

public sealed class AuthorizationEngine : IAuthorizationEngine
{
    private readonly IAuthorizationPolicy _policy;

    public AuthorizationEngine(IAuthorizationPolicy? policy = null)
    {
        _policy = policy ?? new DenyPrecedencePolicy();
    }

    public Task<AuthorizationResult> EvaluateAsync(
        AuthorizationEvaluationContext context,
        IReadOnlyList<AuthorizationRule> rules,
        IReadOnlyCollection<Resource> resources,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(rules);
        ArgumentNullException.ThrowIfNull(resources);

        var resourcesById = resources.ToDictionary(r => r.Id);
        var appliedRules = FindAppliedRules(context, rules, resourcesById);

        if (!appliedRules.Any())
        {
            return Task.FromResult(AuthorizationResult.Denied(context.ResourceId, context.OperationId, Array.Empty<AuthorizationRule>()));
        }

        var resolvedDecisions = appliedRules.Select(r => r.Decision).ToList();
        var finalDecision = _policy.Aggregate(resolvedDecisions);
        var resolvedResourceId = appliedRules.First().ResourceId;

        var result = finalDecision == AuthorizationRuleDecision.Allow
            ? AuthorizationResult.Authorized(resolvedResourceId, context.OperationId, appliedRules)
            : AuthorizationResult.Denied(resolvedResourceId, context.OperationId, appliedRules);

        return Task.FromResult(result);
    }

    private static List<AuthorizationRule> FindAppliedRules(
        AuthorizationEvaluationContext context,
        IReadOnlyList<AuthorizationRule> allRules,
        IReadOnlyDictionary<string, Resource> resourcesById)
    {
        var applied = new List<AuthorizationRule>();
        var visited = new HashSet<string>();

        void Process(string resourceId)
        {
            if (visited.Contains(resourceId)) return;
            visited.Add(resourceId);

            if (!resourcesById.TryGetValue(resourceId, out var resource)) return;

            foreach (var rule in allRules)
            {
                if (!rule.IsActive) continue;
                if (rule.ResourceId != resource.Id || rule.OperationId != context.OperationId) continue;
                if (!RuleMatchesContext(rule, context)) continue;

                if (rule.Decision is AuthorizationRuleDecision.Allow or AuthorizationRuleDecision.Deny)
                {
                    applied.Add(rule);
                }
                else if (rule.Decision == AuthorizationRuleDecision.Inherit)
                {
                    if (resource.ParentId != null)
                    {
                        Process(resource.ParentId);
                    }
                }
            }
        }

        Process(context.ResourceId);
        return applied;
    }

    private static bool RuleMatchesContext(AuthorizationRule rule, AuthorizationEvaluationContext context)
    {
        return rule.AppliesToUserAndRole && rule.UserId == context.UserId && context.RoleIds.Contains(rule.RoleId!)
            || rule.AppliesToUserOnly && rule.UserId == context.UserId
            || rule.AppliesToRoleOnly && context.RoleIds.Contains(rule.RoleId!);
    }
}
