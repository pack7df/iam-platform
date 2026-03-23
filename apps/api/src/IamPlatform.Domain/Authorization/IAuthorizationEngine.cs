namespace IamPlatform.Domain.Authorization;

public interface IAuthorizationEngine
{
    Task<AuthorizationResult> EvaluateAsync(
        AuthorizationEvaluationContext context,
        IReadOnlyList<AuthorizationRule> rules,
        IReadOnlyCollection<Resource> resources,
        CancellationToken cancellationToken = default);
}
