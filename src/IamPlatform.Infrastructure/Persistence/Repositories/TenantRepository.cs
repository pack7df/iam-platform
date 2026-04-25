using IamPlatform.Domain.Tenants;
using IamPlatform.Domain.Users;
using IamPlatform.Domain.Applications;
using IamPlatform.Domain.Operations;
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

    public IApplicationRepository GetApplicationRepository(Guid tenantId)
    {
        return new ApplicationRepository(_context, tenantId);
    }

    public IOperationRepository GetOperationRepository(Guid tenantId)
    {
        return new OperationRepository(_context, tenantId);
    }

    public IRoleRepository GetRoleRepository(Guid tenantId)
    {
        return new RoleRepository(_context, tenantId);
    }
}




