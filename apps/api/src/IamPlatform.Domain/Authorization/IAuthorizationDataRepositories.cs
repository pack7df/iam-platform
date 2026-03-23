using IamPlatform.Domain.Authorization;

namespace IamPlatform.Domain.Authorization;

public interface IResourceRepository
{
    Task<IReadOnlyCollection<Resource>> GetAllForApplicationAsync(string applicationId, CancellationToken cancellationToken = default);
    Task<Resource?> GetByIdAsync(string resourceId, CancellationToken cancellationToken = default);
}

public interface IAuthorizationRuleRepository
{
    Task<IReadOnlyCollection<AuthorizationRule>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<AuthorizationRule>> GetByRoleIdsAsync(IEnumerable<string> roleIds, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<AuthorizationRule>> GetApplicableRulesAsync(
        string userId,
        IEnumerable<string> roleIds,
        string resourceId,
        string operationId,
        CancellationToken cancellationToken = default);
}