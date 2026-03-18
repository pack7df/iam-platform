namespace IamPlatform.Domain.Identity;

public sealed class SystemUserBootstrapper
{
    public SystemUser BootstrapFirstSystemUser(IEnumerable<SystemUser> existingUsers, string systemUserId)
    {
        ArgumentNullException.ThrowIfNull(existingUsers);

        if (existingUsers.Any())
        {
            throw new InvalidOperationException("The first system user has already been bootstrapped.");
        }

        return SystemUser.Create(systemUserId);
    }
}
