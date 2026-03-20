namespace IamPlatform.Domain.Authorization;

public sealed class AuthorizationRuleInheritanceResolver
{
    public AuthorizationInheritanceResolution Resolve(
        AuthorizationRule rule,
        IReadOnlyCollection<AuthorizationRule> rules,
        IReadOnlyCollection<Resource> resources)
    {
        ArgumentNullException.ThrowIfNull(rule);
        ArgumentNullException.ThrowIfNull(rules);
        ArgumentNullException.ThrowIfNull(resources);

        if (rule.Decision.IsResolved())
        {
            return AuthorizationInheritanceResolution.Resolved(rule.Decision, rule.ResourceId);
        }

        var resourcesById = resources.ToDictionary(resource => resource.Id);

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
                && candidate.TenantUserId == rule.TenantUserId
                && candidate.RoleId == rule.RoleId);

            if (parentRule is not null)
            {
                if (parentRule.Decision.IsResolved())
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
