using FluentAssertions;
using IamPlatform.Domain.Authorization;
using Xunit;

namespace IamPlatform.UnitTests.Authorization;

public sealed class AuthorizationRuleDecisionTests
{
    [Theory]
    [InlineData(AuthorizationRuleDecision.Allow)]
    [InlineData(AuthorizationRuleDecision.Deny)]
    public void IsExplicit_Should_Return_True_For_Explicit_Decisions(AuthorizationRuleDecision decision)
    {
        decision.IsExplicit().Should().BeTrue();
        decision.IsResolved().Should().BeTrue();
        decision.IsInherited().Should().BeFalse();
        decision.RequiresParentResolution().Should().BeFalse();
    }

    [Fact]
    public void Inherit_Should_Require_Parent_Resolution_And_Not_Be_Resolved()
    {
        var decision = AuthorizationRuleDecision.Inherit;

        decision.IsInherited().Should().BeTrue();
        decision.RequiresParentResolution().Should().BeTrue();
        decision.IsExplicit().Should().BeFalse();
        decision.IsResolved().Should().BeFalse();
    }
}
