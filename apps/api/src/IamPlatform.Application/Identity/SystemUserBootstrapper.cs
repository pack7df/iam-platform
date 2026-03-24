using IamPlatform.Domain.Identity;

namespace IamPlatform.Application.Identity;

public sealed class SystemUserBootstrapper
{
    private readonly ISystemUserRepository _systemUserRepository;

    public SystemUserBootstrapper(ISystemUserRepository systemUserRepository)
    {
        _systemUserRepository = systemUserRepository;
    }

    public async Task<SystemUserBootstrapResult> BootstrapAsync(
        string systemUserId,
        CancellationToken cancellationToken = default)
    {
        if (await _systemUserRepository.ExistsAnySystemUserAsync(cancellationToken))
        {
            return SystemUserBootstrapResult.AlreadyBootstrapped();
        }

        var systemUser = SystemUser.Create(systemUserId);
        await _systemUserRepository.AddAsync(systemUser, cancellationToken);

        return SystemUserBootstrapResult.Created(systemUser);
    }
}
