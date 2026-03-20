using FluentAssertions;
using IamPlatform.Domain.Authorization;
using IamPlatform.Domain.Tenants;
using Xunit;

namespace IamPlatform.UnitTests.Authorization;

public sealed class AuthorizationRuleMatcherTests
{
    private readonly AuthorizationRuleMatcher _matcher = new();

    [Fact]
    public void IsApplicable_Should_Match_Rule_For_TenantUser_Only()
    {
        var rule = CreateTenantUserRule();
        var context = new AuthorizationEvaluationContext(
            "tenant-user-001",
            "resource-001",
            "operation-001",
            Array.Empty<string>());

        var result = _matcher.IsApplicable(rule, context);

        result.Should().BeTrue();
    }

    [Fact]
    public void IsApplicable_Should_Match_Rule_For_Role_Only()
    {
        var rule = CreateRoleRule();
        var context = new AuthorizationEvaluationContext(
            "tenant-user-999",
            "resource-001",
            "operation-001",
            new[] { "role-001", "role-002" });

        var result = _matcher.IsApplicable(rule, context);

        result.Should().BeTrue();
    }

    [Fact]
    public void IsApplicable_Should_Match_Rule_For_TenantUser_And_Role_When_Both_Match()
    {
        var rule = CreateTenantUserAndRoleRule();
        var context = new AuthorizationEvaluationContext(
            "tenant-user-001",
            "resource-001",
            "operation-001",
            new[] { "role-001" });

        var result = _matcher.IsApplicable(rule, context);

        result.Should().BeTrue();
    }

    [Fact]
    public void IsApplicable_Should_Reject_When_Resource_Does_Not_Match()
    {
        var rule = CreateTenantUserRule();
        var context = new AuthorizationEvaluationContext(
            "tenant-user-001",
            "resource-999",
            "operation-001",
            Array.Empty<string>());

        var result = _matcher.IsApplicable(rule, context);

        result.Should().BeFalse();
    }

    [Fact]
    public void IsApplicable_Should_Reject_When_Operation_Does_Not_Match()
    {
        var rule = CreateTenantUserRule();
        var context = new AuthorizationEvaluationContext(
            "tenant-user-001",
            "resource-001",
            "operation-999",
            Array.Empty<string>());

        var result = _matcher.IsApplicable(rule, context);

        result.Should().BeFalse();
    }

    [Fact]
    public void IsApplicable_Should_Reject_TenantUser_Rule_When_User_Does_Not_Match()
    {
        var rule = CreateTenantUserRule();
        var context = new AuthorizationEvaluationContext(
            "tenant-user-999",
            "resource-001",
            "operation-001",
            Array.Empty<string>());

        var result = _matcher.IsApplicable(rule, context);

        result.Should().BeFalse();
    }

    [Fact]
    public void IsApplicable_Should_Reject_Role_Rule_When_Role_Does_Not_Match()
    {
        var rule = CreateRoleRule();
        var context = new AuthorizationEvaluationContext(
            "tenant-user-001",
            "resource-001",
            "operation-001",
            new[] { "role-999" });

        var result = _matcher.IsApplicable(rule, context);

        result.Should().BeFalse();
    }

    [Fact]
    public void IsApplicable_Should_Reject_Mixed_Rule_When_Only_User_Matches()
    {
        var rule = CreateTenantUserAndRoleRule();
        var context = new AuthorizationEvaluationContext(
            "tenant-user-001",
            "resource-001",
            "operation-001",
            new[] { "role-999" });

        var result = _matcher.IsApplicable(rule, context);

        result.Should().BeFalse();
    }

    [Fact]
    public void IsApplicable_Should_Reject_Mixed_Rule_When_Only_Role_Matches()
    {
        var rule = CreateTenantUserAndRoleRule();
        var context = new AuthorizationEvaluationContext(
            "tenant-user-999",
            "resource-001",
            "operation-001",
            new[] { "role-001" });

        var result = _matcher.IsApplicable(rule, context);

        result.Should().BeFalse();
    }

    [Fact]
    public void IsApplicable_Should_Reject_Inactive_Rule()
    {
        var rule = CreateTenantUserRule();
        rule.Deactivate();
        var context = new AuthorizationEvaluationContext(
            "tenant-user-001",
            "resource-001",
            "operation-001",
            Array.Empty<string>());

        var result = _matcher.IsApplicable(rule, context);

        result.Should().BeFalse();
    }

    private static AuthorizationRule CreateTenantUserRule()
    {
        var tenantUser = TenantUser.Create("tenant-user-001", "tenant-001", TenantUserType.EndUser);
        var resource = Resource.CreateRoot("resource-001", "app-001", "Dashboard", "dashboard");
        var operation = Operation.Create("operation-001", "app-001", "read", "Read");

        return AuthorizationRule.CreateForTenantUser(
            "rule-001",
            tenantUser,
            resource,
            operation,
            AuthorizationRuleDecision.Allow);
    }

    private static AuthorizationRule CreateRoleRule()
    {
        var role = Role.Create("role-001", "tenant-001", "Operators");
        var resource = Resource.CreateRoot("resource-001", "app-001", "Dashboard", "dashboard");
        var operation = Operation.Create("operation-001", "app-001", "read", "Read");

        return AuthorizationRule.CreateForRole(
            "rule-001",
            role,
            resource,
            operation,
            AuthorizationRuleDecision.Allow);
    }

    private static AuthorizationRule CreateTenantUserAndRoleRule()
    {
        var tenantUser = TenantUser.Create("tenant-user-001", "tenant-001", TenantUserType.EndUser);
        var role = Role.Create("role-001", "tenant-001", "Operators");
        var resource = Resource.CreateRoot("resource-001", "app-001", "Dashboard", "dashboard");
        var operation = Operation.Create("operation-001", "app-001", "read", "Read");

        return AuthorizationRule.CreateForTenantUserAndRole(
            "rule-001",
            tenantUser,
            role,
            resource,
            operation,
            AuthorizationRuleDecision.Allow);
    }
}
