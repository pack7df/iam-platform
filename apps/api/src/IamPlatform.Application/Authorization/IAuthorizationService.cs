using IamPlatform.Domain.Authorization;

namespace IamPlatform.Application.Authorization;

public interface IAuthorizationService
{
    Task<AuthorizationResult> EvaluateAsync(
        string userId,
        string resourceId,
        string operationId,
        CancellationToken cancellationToken = default);
}