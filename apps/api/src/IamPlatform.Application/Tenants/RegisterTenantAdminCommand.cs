using IamPlatform.Domain.Common;
using IamPlatform.Domain.Tenants;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace IamPlatform.Application.Tenants;

public sealed record RegisterTenantAdminCommand(string TenantId, string TenantName, string TenantAdminId) : IRequest<TenantAdminRegistrationResult>;

public sealed class RegisterTenantAdminHandler : IRequestHandler<RegisterTenantAdminCommand, TenantAdminRegistrationResult>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IUserRepository _tenantUserRepository;
    private readonly IUnitOfWork _uow;

    public RegisterTenantAdminHandler(
        ITenantRepository tenantRepository,
        IUserRepository tenantUserRepository,
        IUnitOfWork uow)
    {
        _tenantRepository = tenantRepository;
        _tenantUserRepository = tenantUserRepository;
        _uow = uow;
    }

    public async Task<TenantAdminRegistrationResult> Handle(RegisterTenantAdminCommand request, CancellationToken cancellationToken)
    {
        var tenant = Tenant.Create(request.TenantId, request.TenantName);
        var tenantAdmin = User.Create(request.TenantAdminId, tenant.Id, UserType.TenantAdmin);

        await _tenantRepository.AddAsync(tenant, cancellationToken);
        await _tenantUserRepository.AddAsync(tenantAdmin, cancellationToken);

        await _uow.SaveChangesAsync(cancellationToken);

        return new TenantAdminRegistrationResult(tenant, tenantAdmin);
    }
}
