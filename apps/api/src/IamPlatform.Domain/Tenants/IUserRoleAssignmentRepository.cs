using IamPlatform.Domain.Tenants;

namespace IamPlatform.Domain.Tenants;

public interface IUserRoleAssignmentRepository
{
    Task<IReadOnlyCollection<UserRoleAssignment>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task AddAsync(UserRoleAssignment assignment, CancellationToken cancellationToken = default);
    Task RemoveAsync(UserRoleAssignment assignment, CancellationToken cancellationToken = default);
}