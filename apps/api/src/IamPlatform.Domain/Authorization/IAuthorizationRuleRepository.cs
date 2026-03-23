using IamPlatform.Domain.Authorization;

namespace IamPlatform.Domain.Authorization;

public interface IAuthorizationRuleRepository
{
    Task<IReadOnlyCollection<AuthorizationRule>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<AuthorizationRule>> GetByRoleIdsAsync(IEnumerable<string> roleIds, CancellationToken cancellationToken = default);
}