using FluentAssertions;
using IamPlatform.Domain.Authorization;
using Xunit;

namespace IamPlatform.UnitTests.Authorization;

public sealed class DenyPrecedencePolicyTests
{
    private readonly DenyPrecedencePolicy _policy = new();

    [Fact]
    public void Aggregate_Should_Return_Deny_When_NoDecisions()
    {
        var decisions = Array.Empty<AuthorizationRuleDecision>();

        var result = _policy.Aggregate(decisions);

        result.Should().Be(AuthorizationRuleDecision.Deny);
    }

    [Fact]
    public void Aggregate_Should_Return_Deny_When_MixedDecisions_Contain_Deny()
    {
        var decisions = new[]
        {
            AuthorizationRuleDecision.Allow,
            AuthorizationRuleDecision.Deny,
            AuthorizationRuleDecision.Allow
        };

        var result = _policy.Aggregate(decisions);

        result.Should().Be(AuthorizationRuleDecision.Deny);
    }

    [Fact]
    public void Aggregate_Should_Return_Deny_When_AllDecisions_Are_Deny()
    {
        var decisions = new[]
        {
            AuthorizationRuleDecision.Deny,
            AuthorizationRuleDecision.Deny
        };

        var result = _policy.Aggregate(decisions);

        result.Should().Be(AuthorizationRuleDecision.Deny);
    }

    [Fact]
    public void Aggregate_Should_Return_Allow_When_AllDecisions_Are_Allow()
    {
        var decisions = new[]
        {
            AuthorizationRuleDecision.Allow,
            AuthorizationRuleDecision.Allow,
            AuthorizationRuleDecision.Allow
        };

        var result = _policy.Aggregate(decisions);

        result.Should().Be(AuthorizationRuleDecision.Allow);
    }

    [Fact]
    public void Aggregate_Should_Return_Deny_When_SingleDecision_Is_Deny()
    {
        var decisions = new[] { AuthorizationRuleDecision.Deny };

        var result = _policy.Aggregate(decisions);

        result.Should().Be(AuthorizationRuleDecision.Deny);
    }

    [Fact]
    public void Aggregate_Should_Return_Allow_When_SingleDecision_Is_Allow()
    {
        var decisions = new[] { AuthorizationRuleDecision.Allow };

        var result = _policy.Aggregate(decisions);

        result.Should().Be(AuthorizationRuleDecision.Allow);
    }

    [Fact]
    public void Aggregate_Should_Return_Deny_When_Multiple_Deny_Outweigh_Allow()
    {
        var decisions = new[]
        {
            AuthorizationRuleDecision.Allow,
            AuthorizationRuleDecision.Allow,
            AuthorizationRuleDecision.Deny,
            AuthorizationRuleDecision.Allow,
            AuthorizationRuleDecision.Deny
        };

        var result = _policy.Aggregate(decisions);

        result.Should().Be(AuthorizationRuleDecision.Deny);
    }
}