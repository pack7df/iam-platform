using IamPlatform.Domain.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IamPlatform.Infrastructure.Persistence.Repositories;

public sealed class OperationRepository : IOperationRepository
{
    private readonly IamPlatformDbContext _context;

    public OperationRepository(IamPlatformDbContext context)
    {
        _context = context;
    }

    public async Task<Operation?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _context.Operations
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Operation>> GetAllForApplicationAsync(string applicationId, CancellationToken cancellationToken = default)
    {
        return await _context.Operations
            .Where(o => o.ApplicationId == applicationId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Operation operation, CancellationToken cancellationToken = default)
    {
        await _context.Operations.AddAsync(operation, cancellationToken);
    }

    public async Task UpdateAsync(Operation operation, CancellationToken cancellationToken = default)
    {
        _context.Operations.Update(operation);
        await Task.CompletedTask;
    }

    public async Task RemoveAsync(Operation operation, CancellationToken cancellationToken = default)
    {
        _context.Operations.Remove(operation);
        await Task.CompletedTask;
    }
}
