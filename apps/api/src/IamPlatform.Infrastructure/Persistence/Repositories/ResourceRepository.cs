using IamPlatform.Domain.Authorization;
using Microsoft.EntityFrameworkCore;

namespace IamPlatform.Infrastructure.Persistence.Repositories;

public sealed class ResourceRepository : IResourceRepository
{
    private readonly IamPlatformDbContext _context;

    public ResourceRepository(IamPlatformDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<Resource>> GetAllForApplicationAsync(string applicationId, CancellationToken cancellationToken = default)
    {
        return await _context.Resources
            .Where(r => r.ApplicationId == applicationId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Resource?> GetByIdAsync(string resourceId, CancellationToken cancellationToken = default)
    {
        return await _context.Resources
            .FirstOrDefaultAsync(r => r.Id == resourceId, cancellationToken);
    }

    public async Task AddAsync(Resource resource, CancellationToken cancellationToken = default)
    {
        await _context.Resources.AddAsync(resource, cancellationToken);
    }

    public Task UpdateAsync(Resource resource, CancellationToken cancellationToken = default)
    {
        _context.Resources.Update(resource);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(Resource resource, CancellationToken cancellationToken = default)
    {
        _context.Resources.Remove(resource);
        return Task.CompletedTask;
    }
}
