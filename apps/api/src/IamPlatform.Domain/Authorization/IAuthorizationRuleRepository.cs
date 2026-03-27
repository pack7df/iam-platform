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

    Task AddAsync(AuthorizationRule rule, CancellationToken cancellationToken = default);
    Task UpdateAsync(AuthorizationRule rule, CancellationToken cancellationToken = default);
}
