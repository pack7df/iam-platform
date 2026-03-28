using System.Security.Claims;
using FluentAssertions;
using IamPlatform.Domain.Tenants;
using IamPlatform.Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace IamPlatform.UnitTests.Infrastructure.Security;

public sealed class CurrentUserContextTests
{
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly CurrentUserContext _sut;

    public CurrentUserContextTests()
    {
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _sut = new CurrentUserContext(_httpContextAccessorMock.Object);
    }

    [Fact]
    public void UserId_Should_Return_Identifier_Claim()
    {
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, "user-123") };
        SetupHttpContext(claims);

        _sut.UserId.Should().Be("user-123");
    }

    [Fact]
    public void TenantId_Should_Return_Tenant_Claim()
    {
        var claims = new[] { new Claim("tenant_id", "tenant-456") };
        SetupHttpContext(claims);

        _sut.TenantId.Should().Be("tenant-456");
    }

    [Fact]
    public void UserType_Should_Return_Parsed_Role_Claim()
    {
        var claims = new[] { new Claim(ClaimTypes.Role, "TenantAdmin") };
        SetupHttpContext(claims);

        _sut.UserType.Should().Be(UserType.TenantAdmin);
    }

    [Fact]
    public void IsAuthenticated_Should_Return_True_When_User_Is_Authenticated()
    {
        var identityMock = new Mock<ClaimsIdentity>();
        identityMock.Setup(i => i.IsAuthenticated).Returns(true);
        var principal = new ClaimsPrincipal(identityMock.Object);
        
        var context = new DefaultHttpContext { User = principal };
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);

        _sut.IsAuthenticated.Should().BeTrue();
    }

    private void SetupHttpContext(IEnumerable<Claim> claims)
    {
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        var context = new DefaultHttpContext { User = principal };
        
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);
    }
}
