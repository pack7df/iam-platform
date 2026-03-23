using IamPlatform.Domain.Authorization;
using IamPlatform.Domain.Tenants;
using System.Collections.Frozen;

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

        // 1. Find applicable rules (matching)
        var applicable = FindApplicableRules(context, rules);

        // 2. Resolve decisions (direct or inherited)
        var resolvedDecisions = new List<AuthorizationRuleDecision>();
        var appliedRules = new List<AuthorizationRule>();
        var resourcesById = resources.ToDictionary(r => r.Id);
        
        foreach (var rule in applicable)
        {
            if (rule.Decision is AuthorizationRuleDecision.Allow or AuthorizationRuleDecision.Deny)
            {
                resolvedDecisions.Add(rule.Decision);
                appliedRules.Add(rule);
            }
            else if (rule.Decision == AuthorizationRuleDecision.Inherit)
            {
                var resolved = ResolveInheritance(rule, rules, resourcesById);
                if (resolved.IsResolved)
                {
                    resolvedDecisions.Add(resolved.Decision!.Value);
                    // Find the rule that provided the decision (from the parent resource)
                    var resolvedRule = rules.FirstOrDefault(r =>
                        r.ResourceId == resolved.ResolvedResourceId &&
                        r.OperationId == rule.OperationId &&
                        r.UserId == rule.UserId &&
                        r.RoleId == rule.RoleId);
                    if (resolvedRule != null)
                    {
                        appliedRules.Add(resolvedRule);
                    }
                }
            }
        }

        // 3. Aggregate decisions
        var finalDecision = _policy.Aggregate(resolvedDecisions);

        // 4. Determine resolved resource (most specific applied rule)
        var resolvedResourceId = appliedRules.FirstOrDefault()?.ResourceId;

        // 5. Build result
        var result = finalDecision == AuthorizationRuleDecision.Allow
            ? AuthorizationResult.Authorized(resolvedResourceId, appliedRules)
            : AuthorizationResult.Denied(resolvedResourceId, appliedRules);

        return Task.FromResult(result);
    }

    private static List<AuthorizationRule> FindApplicableRules(
        AuthorizationEvaluationContext context,
        IReadOnlyList<AuthorizationRule> rules)
    {
        var applicable = new List<AuthorizationRule>();

        foreach (var rule in rules)
        {
            if (!rule.IsActive) continue;
            if (rule.ResourceId != context.ResourceId || rule.OperationId != context.OperationId) continue;

            if (rule.AppliesToUserAndRole)
            {
                if (rule.UserId == context.UserId && context.RoleIds.Contains(rule.RoleId!))
                    applicable.Add(rule);
            }
            else if (rule.AppliesToUserOnly)
            {
                if (rule.UserId == context.UserId)
                    applicable.Add(rule);
            }
            else if (rule.AppliesToRoleOnly)
            {
                if (context.RoleIds.Contains(rule.RoleId!))
                    applicable.Add(rule);
            }
        }

        return applicable;
    }

    private static AuthorizationInheritanceResolution ResolveInheritance(
        AuthorizationRule rule,
        IReadOnlyList<AuthorizationRule> rules,
        IReadOnlyDictionary<string, Resource> resourcesById)
    {
        if (!resourcesById.TryGetValue(rule.ResourceId, out var currentResource))
        {
            return AuthorizationInheritanceResolution.Unresolved();
        }

        while (currentResource.ParentId is not null)
        {
            if (!resourcesById.TryGetValue(currentResource.ParentId, out var parentResource))
            {
                return AuthorizationInheritanceResolution.Unresolved();
            }

            var parentRule = rules.FirstOrDefault(candidate =>
                candidate.IsActive
                && candidate.ResourceId == parentResource.Id
                && candidate.OperationId == rule.OperationId
                && candidate.UserId == rule.UserId
                && candidate.RoleId == rule.RoleId);

            if (parentRule is not null)
            {
                if (parentRule.Decision is AuthorizationRuleDecision.Allow or AuthorizationRuleDecision.Deny)
                {
                    return AuthorizationInheritanceResolution.Resolved(parentRule.Decision, parentResource.Id);
                }

                currentResource = parentResource;
                continue;
            }

            currentResource = parentResource;
        }

        return AuthorizationInheritanceResolution.Unresolved();
    }
}