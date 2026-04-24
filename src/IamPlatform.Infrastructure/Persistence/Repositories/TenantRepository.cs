using IamPlatform.Domain.Tenants;
using IamPlatform.Domain.Users;
using IamPlatform.Infrastructure.Persistence.Repositories;

namespace IamPlatform.Infrastructure.Persistence.Repositories;

public class TenantRepository : BaseRepository<Tenant>, ITenantRepository
{
    public TenantRepository(IamPlatformDbContext context) : base(context)
    {
    }

    public IUserRepository GetUserRepository(Guid tenantId)
    {
        return new UserRepository(_context, tenantId);
    }
}
