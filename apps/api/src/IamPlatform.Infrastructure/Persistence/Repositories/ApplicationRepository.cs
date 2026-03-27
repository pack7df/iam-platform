using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IamPlatform.Domain.Tenants;
using Microsoft.EntityFrameworkCore;

namespace IamPlatform.Infrastructure.Persistence.Repositories;

public sealed class ApplicationRepository : IApplicationRepository
{
    private readonly IamPlatformDbContext _context;

    public ApplicationRepository(IamPlatformDbContext context)
    {
        _context = context;
    }

    public async Task<IamPlatform.Domain.Tenants.Application?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _context.Applications.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IReadOnlyCollection<IamPlatform.Domain.Tenants.Application>> GetByTenantIdAsync(string tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.Applications
            .Where(a => a.TenantId == tenantId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(IamPlatform.Domain.Tenants.Application application, CancellationToken cancellationToken = default)
    {
        await _context.Applications.AddAsync(application, cancellationToken);
    }

    public Task UpdateAsync(IamPlatform.Domain.Tenants.Application application, CancellationToken cancellationToken = default)
    {
        _context.Applications.Update(application);
        return Task.CompletedTask;
    }
}
