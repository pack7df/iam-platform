using IamPlatform.Domain.Identity;

namespace IamPlatform.Application.Identity;

public interface ISystemUserBootstrapper
{
    Task<SystemUserBootstrapResult> BootstrapAsync(
        string systemUserId,
        CancellationToken cancellationToken = default);
}
