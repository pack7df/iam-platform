using IamPlatform.Domain.Common;
using IamPlatform.Domain.Users;

namespace IamPlatform.Domain.Tenants;

public interface ITenantRepository : IRepository<Tenant>
{
    IUserRepository GetUserRepository(Guid tenantId);
}

