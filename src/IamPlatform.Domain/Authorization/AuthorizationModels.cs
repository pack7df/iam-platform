namespace IamPlatform.Domain.Authorization;

public record AuthorizationRequest(
    Guid UserId,
    Guid ResourceId,
    Guid OperationId
);


public record AuthorizationResponse(
    PermissionOutcome Decision,
    string? Reason = null,
    DateTime EvaluatedAt = default
)
{
    public static AuthorizationResponse Allow(string? reason = null) 
        => new(PermissionOutcome.Allowed, reason, DateTime.UtcNow);

    public static AuthorizationResponse Deny(string? reason = null) 
        => new(PermissionOutcome.Denied, reason, DateTime.UtcNow);
        
    public bool IsAllowed => Decision == PermissionOutcome.Allowed;
}
