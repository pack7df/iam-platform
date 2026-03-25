using IamPlatform.Domain.Common;
using IamPlatform.Domain.Identity;
using IamPlatform.Domain.Tenants;
using System.Threading;
using System.Threading.Tasks;

namespace IamPlatform.Application.Identity;

public sealed class SystemUserBootstrapper : ISystemUserBootstrapper
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _uow;

    public SystemUserBootstrapper(IUserRepository userRepository, IUnitOfWork uow)
    {
        _userRepository = userRepository;
        _uow = uow;
    }

    public async Task<SystemUserBootstrapResult> BootstrapAsync(
        string systemUserId,
        CancellationToken cancellationToken = default)
    {
        if (await _userRepository.ExistsByTypeAsync(UserType.SystemUser, cancellationToken))
        {
            return SystemUserBootstrapResult.AlreadyBootstrapped();
        }

        var systemUser = User.CreateSystemUser(systemUserId);
        await _userRepository.AddAsync(systemUser, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return SystemUserBootstrapResult.Created(systemUser);
    }
}
