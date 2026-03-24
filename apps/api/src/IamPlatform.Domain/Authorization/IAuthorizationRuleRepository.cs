using IamPlatform.Domain.Authorization;

namespace IamPlatform.Domain.Authorization;

public interface IAuthorizationRuleRepository
{
    Task<IReadOnlyCollection<AuthorizationRule>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<AuthorizationRule>> GetApplicableRulesAsync(
        string userId,
        string resourceId,
        string operationId,
        CancellationToken cancellationToken = default);
}
