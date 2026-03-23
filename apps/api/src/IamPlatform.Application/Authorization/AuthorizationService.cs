using IamPlatform.Domain.Authorization;

namespace IamPlatform.Application.Authorization;

public sealed class AuthorizationService : IAuthorizationService
{
    private readonly IAuthorizationEngine _engine;
    private readonly IResourceRepository _resourceRepository;
    private readonly IAuthorizationRuleRepository _ruleRepository;

    public AuthorizationService(
        IAuthorizationEngine engine,
        IResourceRepository resourceRepository,
        IAuthorizationRuleRepository ruleRepository)
    {
        _engine = engine;
        _resourceRepository = resourceRepository;
        _ruleRepository = ruleRepository;
    }

    public async Task<AuthorizationResult> EvaluateAsync(
        string userId,
        string resourceId,
        string operationId,
        IEnumerable<string> roleIds,
        CancellationToken cancellationToken = default)
    {
        // 1. Get resource
        var resource = await _resourceRepository.GetByIdAsync(resourceId, cancellationToken);
        if (resource == null)
        {
            return AuthorizationResult.Denied(null, Array.Empty<AuthorizationRule>());
        }

        // 2. Get all resources for the application (for inheritance resolution)
        var allResources = await _resourceRepository.GetAllForApplicationAsync(resource.ApplicationId, cancellationToken);

        // 3. Get rules
        var userRules = await _ruleRepository.GetByUserIdAsync(userId, cancellationToken);
        var roleRules = await _ruleRepository.GetByRoleIdsAsync(roleIds, cancellationToken);
        var allRules = userRules.Concat(roleRules).ToList();

        // 4. Build context
        var context = new AuthorizationEvaluationContext(
            userId,
            resourceId,
            operationId,
            roleIds?.ToArray() ?? Array.Empty<string>());

        // 5. Evaluate
        return await _engine.EvaluateAsync(context, allRules, allResources, cancellationToken);
    }
}