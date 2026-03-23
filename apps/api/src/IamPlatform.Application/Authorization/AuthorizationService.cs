using IamPlatform.Domain.Authorization;

namespace IamPlatform.Application.Authorization;

public sealed class AuthorizationService : IAuthorizationService
{
    private readonly IAuthorizationEngine _engine;

    public AuthorizationService(IAuthorizationEngine engine)
    {
        _engine = engine;
    }

    public Task<AuthorizationResult> EvaluateAsync(
        string userId,
        string resourceId,
        string operationId,
        CancellationToken cancellationToken = default)
    {
        return _engine.EvaluateAsync(userId, resourceId, operationId, cancellationToken);
    }
}
