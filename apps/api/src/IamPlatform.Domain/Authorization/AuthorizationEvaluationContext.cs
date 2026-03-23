using IamPlatform.Domain.Primitives;

namespace IamPlatform.Domain.Authorization;

public sealed class AuthorizationEvaluationContext
{
    public AuthorizationEvaluationContext(
        string userId,
        string resourceId,
        string operationId,
        IReadOnlyCollection<string> roleIds)
    {
        UserId = Guard.Required(userId, nameof(userId), "User id is required.");
        ResourceId = Guard.Required(resourceId, nameof(resourceId), "Resource id is required.");
        OperationId = Guard.Required(operationId, nameof(operationId), "Operation id is required.");
        RoleIds = roleIds ?? throw new ArgumentNullException(nameof(roleIds));
    }

    public string UserId { get; }

    public string ResourceId { get; }

    public string OperationId { get; }

    public IReadOnlyCollection<string> RoleIds { get; }
}
