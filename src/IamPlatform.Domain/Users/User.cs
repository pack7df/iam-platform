using System.Diagnostics.CodeAnalysis;
using IamPlatform.Domain.Common;

namespace IamPlatform.Domain.Users;

public enum UserStatus
{
    Active,
    Suspended,
    Inactive
}

public class User : BaseEntity, ITenantEntity
{
    public Guid TenantId { get; set; }

    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public UserStatus Status { get; set; } = UserStatus.Active;

    private User() { } // For EF Core

    [SetsRequiredMembers]
    public User(Guid tenantId, string username, string email, string passwordHash)
    {
        TenantId = tenantId;
        Username = username;
        Email = email;
        PasswordHash = passwordHash;
    }
}
