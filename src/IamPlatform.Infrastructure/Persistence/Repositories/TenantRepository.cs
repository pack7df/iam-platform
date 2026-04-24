using IamPlatform.Domain.Tenants;
using IamPlatform.Infrastructure.Persistence.Repositories;

namespace IamPlatform.Infrastructure.Persistence.Repositories;

public class TenantRepository : BaseRepository<Tenant>, ITenantRepository
{
    public TenantRepository(IamPlatformDbContext context) : base(context)
    {
    }
}
