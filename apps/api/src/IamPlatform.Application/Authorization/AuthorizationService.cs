using IamPlatform.Domain.Authorization;
using IamPlatform.Domain.Tenants;

namespace IamPlatform.Application.Authorization;

public sealed class AuthorizationService : IAuthorizationService
{
    private readonly IAuthorizationEngine _engine;
    private readonly IResourceRepository _resourceRepository;
    private readonly IAuthorizationRuleRepository _ruleRepository;
    private readonly IUserRoleAssignmentRepository _userRoleAssignmentRepository;

    public AuthorizationService(
        IAuthorizationEngine engine,
        IResourceRepository resourceRepository,
        IAuthorizationRuleRepository ruleRepository,
        IUserRoleAssignmentRepository userRoleAssignmentRepository)
    {
        _engine = engine;
        _resourceRepository = resourceRepository;
        _ruleRepository = ruleRepository;
        _userRoleAssignmentRepository = userRoleAssignmentRepository;
    }

    public async Task<AuthorizationResult> EvaluateAsync(
        string userId,
        string resourceId,
        string operationId,
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

        // 3. Get user's role assignments and extract role IDs
        var userRoleAssignments = await _userRoleAssignmentRepository.GetByUserIdAsync(userId, cancellationToken);
        var roleIds = userRoleAssignments.Select(ra => ra.RoleId).ToArray();

        // 4. Get rules: user-specific rules + role-based rules
        var userRules = await _ruleRepository.GetByUserIdAsync(userId, cancellationToken);
        var roleRules = await _ruleRepository.GetByRoleIdsAsync(roleIds, cancellationToken);
        var allRules = userRules.Concat(roleRules).ToList();

        // 5. Build context
        var context = new AuthorizationEvaluationContext(
            userId,
            resourceId,
            operationId,
            roleIds);

        // 6. Evaluate
        return await _engine.EvaluateAsync(context, allRules, allResources, cancellationToken);
    }
}