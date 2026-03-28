using IamPlatform.Application.Common.Interfaces;
using IamPlatform.Domain.Tenants;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace IamPlatform.Infrastructure.Security;

public sealed class CurrentUserContext : ICurrentUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId => GetClaimValue(ClaimTypes.NameIdentifier);

    public string? TenantId => GetClaimValue("tenant_id");

    public UserType? UserType
    {
        get
        {
            var typeString = GetClaimValue(ClaimTypes.Role);
            if (string.IsNullOrEmpty(typeString)) return null;
            return Enum.TryParse<UserType>(typeString, out var type) ? type : null;
        }
    }

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    private string? GetClaimValue(string claimType)
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst(claimType)?.Value;
    }
}
