using IamPlatform.Domain.Tenants;
using Microsoft.EntityFrameworkCore;

namespace IamPlatform.Infrastructure.Persistence.Repositories;

public sealed class UserRoleAssignmentRepository : IUserRoleAssignmentRepository
{
    private readonly IamPlatformDbContext _context;

    public UserRoleAssignmentRepository(IamPlatformDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<UserRoleAssignment>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.UserRoleAssignments
            .Where(ra => ra.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(UserRoleAssignment assignment, CancellationToken cancellationToken = default)
    {
        await _context.UserRoleAssignments.AddAsync(assignment, cancellationToken);
    }

    public Task RemoveAsync(UserRoleAssignment assignment, CancellationToken cancellationToken = default)
    {
        _context.UserRoleAssignments.Remove(assignment);
        return Task.CompletedTask;
    }
}
