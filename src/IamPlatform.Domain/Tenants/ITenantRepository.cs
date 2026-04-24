using IamPlatform.Domain.Common;
using IamPlatform.Domain.Users;
using IamPlatform.Domain.Applications;
using IamPlatform.Domain.Operations;

namespace IamPlatform.Domain.Tenants;

public interface ITenantRepository : IRepository<Tenant>
{
    IUserRepository GetUserRepository(Guid tenantId);
    IApplicationRepository GetApplicationRepository(Guid tenantId);
    IOperationRepository GetOperationRepository(Guid tenantId);
}




