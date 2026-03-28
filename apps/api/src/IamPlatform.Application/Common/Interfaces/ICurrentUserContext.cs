using IamPlatform.Domain.Tenants;

namespace IamPlatform.Application.Common.Interfaces;

public interface ICurrentUserContext
{
    string? UserId { get; }
    string? TenantId { get; }
    UserType? UserType { get; }
    bool IsAuthenticated { get; }
}
