using IamPlatform.Domain.Authorization;
using IamPlatform.Domain.Tenants;
using Microsoft.EntityFrameworkCore;

namespace IamPlatform.Infrastructure.Persistence.Repositories;

public sealed class AuthorizationRuleRepository : IAuthorizationRuleRepository
{
    private readonly IamPlatformDbContext _context;
    private readonly IUserRoleAssignmentRepository _userRoleAssignmentRepository;

    public AuthorizationRuleRepository(
        IamPlatformDbContext context,
        IUserRoleAssignmentRepository userRoleAssignmentRepository)
    {
        _context = context;
        _userRoleAssignmentRepository = userRoleAssignmentRepository;
    }

    public async Task<IReadOnlyCollection<AuthorizationRule>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.AuthorizationRules
            .Where(r => r.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<AuthorizationRule>> GetApplicableRulesAsync(
        string userId,
        string resourceId,
        string operationId,
        CancellationToken cancellationToken = default)
    {
        // Get roles assigned to user to check role-based rules
        var userRoleAssignments = await _userRoleAssignmentRepository.GetByUserIdAsync(userId, cancellationToken);
        var roleIds = userRoleAssignments.Select(ra => ra.RoleId).ToList();

        return await _context.AuthorizationRules
            .Where(r =>
                r.IsActive &&
                r.ResourceId == resourceId &&
                r.OperationId == operationId &&
                (
                    (r.UserId == userId && r.RoleId != null && roleIds.Contains(r.RoleId)) || // Applies to User and Role
                    (r.UserId == userId && r.RoleId == null) ||                             // Applies to User only
                    (r.UserId == null && r.RoleId != null && roleIds.Contains(r.RoleId))     // Applies to Role only
                ))
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(AuthorizationRule rule, CancellationToken cancellationToken = default)
    {
        await _context.AuthorizationRules.AddAsync(rule, cancellationToken);
    }

    public Task UpdateAsync(AuthorizationRule rule, CancellationToken cancellationToken = default)
    {
        _context.AuthorizationRules.Update(rule);
        return Task.CompletedTask;
    }
}
