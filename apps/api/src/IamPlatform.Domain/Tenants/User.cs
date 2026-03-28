using IamPlatform.Domain.Primitives;

namespace IamPlatform.Domain.Tenants;

public sealed class User
{
    private User(string id, string? tenantId, UserType type, UserStatus status)
    {
        Id = Guard.Required(id, nameof(id), "User id is required.");
        
        // Validation: SystemUser can have null tenantId, but TenantUser must have tenantId
        if (type != UserType.SystemUser)
        {
            TenantId = Guard.Required(tenantId, nameof(tenantId), "Tenant id is required for non-system users.");
        }
        else
        {
            // SystemUser: tenantId must be null or empty (normalize to null)
            TenantId = string.IsNullOrWhiteSpace(tenantId) ? null : tenantId.Trim();
        }
        
        Type = type;
        Status = status;
    }

    public string Id { get; }

    public string? TenantId { get; }

    public UserType Type { get; private set; }

    public UserStatus Status { get; private set; }

    public string? PasswordHash { get; private set; }

    public string? Salt { get; private set; }

    public bool IsActive => Status == UserStatus.Active;

    public static User Create(string id, string tenantId, UserType type, UserStatus status = UserStatus.PendingVerification)
    {
        return new User(id, tenantId, type, status);
    }

    public static User CreateSystemUser(string id, UserStatus status = UserStatus.Active)
    {
        return new User(id, null, UserType.SystemUser, status);
    }

    public void ChangeType(UserType type)
    {
        // If changing to non-SystemUser, ensure TenantId is set
        if (type != UserType.SystemUser && string.IsNullOrWhiteSpace(TenantId))
        {
            throw new InvalidOperationException("Cannot change to non-system user without a tenant id.");
        }
        Type = type;
    }

    public void UpdateStatus(UserStatus status)
    {
        Status = status;
    }

    public void SetPassword(string hash, string salt)
    {
        PasswordHash = Guard.Required(hash, nameof(hash), "Password hash is required.");
        Salt = Guard.Required(salt, nameof(salt), "Salt is required.");
    }
}
