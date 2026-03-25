using IamPlatform.Domain.Identity;
using IamPlatform.Domain.Tenants;
using System.Threading;
using System.Threading.Tasks;

namespace IamPlatform.Application.Identity;

public sealed class SystemUserBootstrapper : ISystemUserBootstrapper
{
    private readonly IUserRepository _userRepository;

    public SystemUserBootstrapper(IUserRepository userRepository)
    {
        _userRepository = userRepository;
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

        return SystemUserBootstrapResult.Created(systemUser);
    }
}
