using IamPlatform.Domain.Primitives;

namespace IamPlatform.Domain.Authorization;

public sealed class AuthorizationEvaluationContext
{
    public AuthorizationEvaluationContext(
        string tenantUserId,
        string resourceId,
        string operationId,
        IReadOnlyCollection<string> roleIds)
    {
        TenantUserId = Guard.Required(tenantUserId, nameof(tenantUserId), "Tenant user id is required.");
        ResourceId = Guard.Required(resourceId, nameof(resourceId), "Resource id is required.");
        OperationId = Guard.Required(operationId, nameof(operationId), "Operation id is required.");
        RoleIds = roleIds ?? throw new ArgumentNullException(nameof(roleIds));
    }

    public string TenantUserId { get; }

    public string ResourceId { get; }

    public string OperationId { get; }

    public IReadOnlyCollection<string> RoleIds { get; }
}
