using IamPlatform.Domain.Tenants;
using Microsoft.EntityFrameworkCore;

namespace IamPlatform.Infrastructure.Persistence.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly IamPlatformDbContext _context;

    public UserRepository(IamPlatformDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<User>> GetByTenantIdAsync(string tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Where(u => u.TenantId == tenantId)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByTypeAsync(UserType type, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AnyAsync(u => u.Type == type, cancellationToken);
    }
}
