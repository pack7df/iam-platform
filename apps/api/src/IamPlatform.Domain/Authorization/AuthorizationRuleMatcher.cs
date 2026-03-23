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

        if (rule.AppliesToUserAndRole)
        {
            return rule.UserId == context.UserId
                && context.RoleIds.Contains(rule.RoleId!);
        }

        if (rule.AppliesToUserOnly)
        {
            return rule.UserId == context.UserId;
        }

        if (rule.AppliesToRoleOnly)
        {
            return context.RoleIds.Contains(rule.RoleId!);
        }

        return false;
    }
}
