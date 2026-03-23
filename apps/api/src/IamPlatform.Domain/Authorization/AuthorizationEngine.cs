using IamPlatform.Domain.Authorization;
using IamPlatform.Domain.Tenants;

namespace IamPlatform.Domain.Authorization;

public sealed class AuthorizationEngine : IAuthorizationEngine
{
    private readonly IAuthorizationRuleRepository _ruleRepository;
    private readonly IResourceRepository _resourceRepository;
    private readonly IAuthorizationPolicy _policy;

    public AuthorizationEngine(
        IAuthorizationRuleRepository ruleRepository,
        IResourceRepository resourceRepository,
        IAuthorizationPolicy? policy = null)
    {
        _ruleRepository = ruleRepository;
        _resourceRepository = resourceRepository;
        _policy = policy ?? new DenyPrecedencePolicy();
    }

    public async Task<AuthorizationResult> EvaluateAsync(
        string userId,
        string resourceId,
        string operationId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(userId);
        ArgumentNullException.ThrowIfNull(resourceId);
        ArgumentNullException.ThrowIfNull(operationId);

        // Find applied rules recursively with on-demand data loading
        var appliedRules = await FindAppliedRulesAsync(userId, resourceId, operationId, cancellationToken);

        if (!appliedRules.Any())
        {
            return AuthorizationResult.Denied(resourceId, operationId, Array.Empty<AuthorizationRule>());
        }

        var resolvedDecisions = appliedRules.Select(r => r.Decision).ToList();
        var finalDecision = _policy.Aggregate(resolvedDecisions);
        var resolvedResourceId = appliedRules.First().ResourceId;

        return finalDecision == AuthorizationRuleDecision.Allow
            ? AuthorizationResult.Authorized(resolvedResourceId, operationId, appliedRules)
            : AuthorizationResult.Denied(resolvedResourceId, operationId, appliedRules);
    }

    private async Task<List<AuthorizationRule>> FindAppliedRulesAsync(
        string userId,
        string resourceId,
        string operationId,
        CancellationToken cancellationToken)
    {
        var applied = new List<AuthorizationRule>();
        var visited = new HashSet<string>();

        async Task Process(string currentResourceId)
        {
            if (visited.Contains(currentResourceId)) return;
            visited.Add(currentResourceId);

            // Get applicable rules for this resource directly from repository (on-demand)
            var rules = await _ruleRepository.GetApplicableRulesAsync(
                userId, currentResourceId, operationId, cancellationToken);

            foreach (var rule in rules)
            {
                if (!rule.IsActive) continue;

                switch (rule.Decision)
                {
                    case AuthorizationRuleDecision.Inherit:
                        var resource = await _resourceRepository.GetByIdAsync(currentResourceId, cancellationToken);
                        if (resource?.ParentId != null)
                        {
                            await Process(resource.ParentId);
                        }
                        break;
                    case AuthorizationRuleDecision.Allow:
                    case AuthorizationRuleDecision.Deny:
                        applied.Add(rule);
                        break;
                }
            }
        }

        await Process(resourceId);
        return applied;
    }

}
