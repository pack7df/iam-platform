using FluentAssertions;
using IamPlatform.Domain.Tenants;
using Xunit;

namespace IamPlatform.UnitTests.Tenants;

public sealed class VerificationCodeTests
{
    [Fact]
    public void Create_Should_Set_Properties()
    {
        var userId = "user-001";
        var code = "123456";
        var expiresAt = DateTime.UtcNow.AddMinutes(15);
        var type = VerificationCodeType.Registration;

        var vc = VerificationCode.Create(userId, code, expiresAt, type);

        vc.Id.Should().NotBeNullOrWhiteSpace();
        vc.UserId.Should().Be(userId);
        vc.Code.Should().Be(code);
        vc.ExpiresAt.Should().Be(expiresAt);
        vc.Type.Should().Be(type);
        vc.IsUsed.Should().BeFalse();
    }

    [Fact]
    public void IsExpired_Should_Return_True_When_Past_Expiration()
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(-1);
        var vc = VerificationCode.Create("u1", "c", expiresAt, VerificationCodeType.Registration);

        vc.IsExpired(DateTime.UtcNow).Should().BeTrue();
    }

    [Fact]
    public void Use_Should_Set_IsUsed_To_True()
    {
        var vc = VerificationCode.Create("u1", "c", DateTime.UtcNow.AddMinutes(10), VerificationCodeType.Registration);

        vc.Use();

        vc.IsUsed.Should().BeTrue();
    }

    [Fact]
    public void Use_Should_Throw_If_Already_Used()
    {
        var vc = VerificationCode.Create("u1", "c", DateTime.UtcNow.AddMinutes(10), VerificationCodeType.Registration);
        vc.Use();

        var act = () => vc.Use();

        act.Should().Throw<InvalidOperationException>();
    }
}
