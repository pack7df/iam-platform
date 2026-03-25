using IamPlatform.Domain.Identity;
using Microsoft.EntityFrameworkCore;

namespace IamPlatform.Infrastructure.Persistence.Repositories;

public sealed class InvitationRepository : IInvitationRepository
{
    private readonly IamPlatformDbContext _context;

    public InvitationRepository(IamPlatformDbContext context)
    {
        _context = context;
    }

    public async Task<Invitation?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _context.Invitations
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task AddAsync(Invitation invitation, CancellationToken cancellationToken = default)
    {
        await _context.Invitations.AddAsync(invitation, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Invitation invitation, CancellationToken cancellationToken = default)
    {
        _context.Invitations.Update(invitation);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
