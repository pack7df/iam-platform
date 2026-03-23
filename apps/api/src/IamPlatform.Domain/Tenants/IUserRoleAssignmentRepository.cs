using IamPlatform.Domain.Tenants;

namespace IamPlatform.Domain.Tenants;

public interface IUserRoleAssignmentRepository
{
    Task<IReadOnlyCollection<UserRoleAssignment>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
}