using IamPlatform.Domain.Authorization;

namespace IamPlatform.Application.Authorization;

public interface IAuthorizationService
{
    Task<AuthorizationResult> EvaluateAsync(
        string userId,
        string resourceId,
        string operationId,
        IEnumerable<string> roleIds,
        CancellationToken cancellationToken = default);
}