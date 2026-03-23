namespace IamPlatform.Domain.Authorization;

public interface IAuthorizationEngine
{
    Task<AuthorizationResult> EvaluateAsync(
        string userId,
        string resourceId,
        string operationId,
        CancellationToken cancellationToken = default);
}
