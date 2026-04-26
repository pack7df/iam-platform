using FluentAssertions;
using IamPlatform.Domain.Authorization;
using Xunit;

namespace IamPlatform.UnitTests.Domain.Authorization;

public class AuthorizationModelsTests
{
    [Fact]
    public void AuthorizationResponse_Allow_ShouldSetCorrectDecision()
    {
        // Act
        var response = AuthorizationResponse.Allow("Testing");

        // Assert
        response.Decision.Should().Be(PermissionOutcome.Allowed);
        response.Reason.Should().Be("Testing");
        response.IsAllowed.Should().BeTrue();
        response.EvaluatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void AuthorizationResponse_Deny_ShouldSetCorrectDecision()
    {
        // Act
        var response = AuthorizationResponse.Deny("Security Policy");

        // Assert
        response.Decision.Should().Be(PermissionOutcome.Denied);
        response.Reason.Should().Be("Security Policy");
        response.IsAllowed.Should().BeFalse();
    }

    [Fact]
    public void EvaluationContext_ShouldInitializeCorrectly()
    {
        // Arrange
        var request = new AuthorizationRequest(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        // Act
        var context = new EvaluationContext(request);

        // Assert
        context.Request.Should().Be(request);
    }


}
