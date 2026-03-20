namespace IamPlatform.Domain.Authorization;

public sealed class AuthorizationRuleMatcher
{
    public bool IsApplicable(AuthorizationRule rule, AuthorizationEvaluationContext context)
    {
        ArgumentNullException.ThrowIfNull(rule);
        ArgumentNullException.ThrowIfNull(context);

        if (!rule.IsActive)
        {
            return false;
        }

        if (rule.ResourceId != context.ResourceId || rule.OperationId != context.OperationId)
        {
            return false;
        }

        if (rule.AppliesToTenantUserAndRole)
        {
            return rule.TenantUserId == context.TenantUserId
                && context.RoleIds.Contains(rule.RoleId!);
        }

        if (rule.AppliesToTenantUserOnly)
        {
            return rule.TenantUserId == context.TenantUserId;
        }

        if (rule.AppliesToRoleOnly)
        {
            return context.RoleIds.Contains(rule.RoleId!);
        }

        return false;
    }
}
