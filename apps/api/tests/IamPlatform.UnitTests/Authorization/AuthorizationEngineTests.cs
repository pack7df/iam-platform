using FluentAssertions;
using IamPlatform.Domain.Authorization;
using IamPlatform.Domain.Tenants;
using Xunit;

namespace IamPlatform.UnitTests.Authorization;

public sealed class AuthorizationEngineTests
{
    private readonly IAuthorizationEngine _engine = new AuthorizationEngine();

    [Fact]
    public async Task EvaluateAsync_Should_Allow_When_Single_Allow_Rule_Matches()
    {
        // Arrange
        var user = User.Create("user-001", "tenant-001", UserType.EndUser);
        var resource = Resource.CreateRoot("resource-001", "app-001", "Dashboard", "dashboard");
        var resources = new[] { resource };
        var operation = Operation.Create("operation-001", "app-001", "read", "Read");
        var rule = AuthorizationRule.CreateForUser(
            "rule-001",
            user,
            resource,
            operation,
            AuthorizationRuleDecision.Allow);
        var context = new AuthorizationEvaluationContext(
            "user-001",
            "resource-001",
            "operation-001",
            Array.Empty<string>());

        // Act
        var result = await _engine.EvaluateAsync(context, new[] { rule }, resources);

        // Assert
        result.IsAuthorized.Should().BeTrue();
        result.Decision.Should().Be(AuthorizationRuleDecision.Allow);
        result.ResolvedResourceId.Should().Be("resource-001");
        result.AppliedRules.Should().ContainSingle();
    }

    [Fact]
    public async Task EvaluateAsync_Should_Deny_When_Single_Deny_Rule_Matches()
    {
        // Arrange
        var user = User.Create("user-001", "tenant-001", UserType.EndUser);
        var resource = Resource.CreateRoot("resource-001", "app-001", "Dashboard", "dashboard");
        var resources = new[] { resource };
        var operation = Operation.Create("operation-001", "app-001", "read", "Read");
        var rule = AuthorizationRule.CreateForUser(
            "rule-001",
            user,
            resource,
            operation,
            AuthorizationRuleDecision.Deny);
        var context = new AuthorizationEvaluationContext(
            "user-001",
            "resource-001",
            "operation-001",
            Array.Empty<string>());

        // Act
        var result = await _engine.EvaluateAsync(context, new[] { rule }, resources);

        // Assert
        result.IsAuthorized.Should().BeFalse();
        result.Decision.Should().Be(AuthorizationRuleDecision.Deny);
        result.ResolvedResourceId.Should().Be("resource-001");
    }

    [Fact]
    public async Task EvaluateAsync_Should_Allow_When_Multiple_Allow_Rules_All_Allow()
    {
        // Arrange
        var user = User.Create("user-001", "tenant-001", UserType.EndUser);
        var resource = Resource.CreateRoot("resource-001", "app-001", "Dashboard", "dashboard");
        var resources = new[] { resource };
        var operation = Operation.Create("operation-001", "app-001", "read", "Read");
        var rule1 = AuthorizationRule.CreateForUser("rule-001", user, resource, operation, AuthorizationRuleDecision.Allow);
        var rule2 = AuthorizationRule.CreateForUser("rule-002", user, resource, operation, AuthorizationRuleDecision.Allow);
        var context = new AuthorizationEvaluationContext(
            "user-001",
            "resource-001",
            "operation-001",
            Array.Empty<string>());

        // Act
        var result = await _engine.EvaluateAsync(context, new[] { rule1, rule2 }, resources);

        // Assert
        result.IsAuthorized.Should().BeTrue();
        result.Decision.Should().Be(AuthorizationRuleDecision.Allow);
    }

    [Fact]
    public async Task EvaluateAsync_Should_Deny_When_Any_Deny_Rule_Among_Allows()
    {
        // Arrange
        var user = User.Create("user-001", "tenant-001", UserType.EndUser);
        var resource = Resource.CreateRoot("resource-001", "app-001", "Dashboard", "dashboard");
        var resources = new[] { resource };
        var operation = Operation.Create("operation-001", "app-001", "read", "Read");
        var rule1 = AuthorizationRule.CreateForUser("rule-001", user, resource, operation, AuthorizationRuleDecision.Allow);
        var rule2 = AuthorizationRule.CreateForUser("rule-002", user, resource, operation, AuthorizationRuleDecision.Deny);
        var rule3 = AuthorizationRule.CreateForUser("rule-003", user, resource, operation, AuthorizationRuleDecision.Allow);
        var context = new AuthorizationEvaluationContext(
            "user-001",
            "resource-001",
            "operation-001",
            Array.Empty<string>());

        // Act
        var result = await _engine.EvaluateAsync(context, new[] { rule1, rule2, rule3 }, resources);

        // Assert
        result.IsAuthorized.Should().BeFalse();
        result.Decision.Should().Be(AuthorizationRuleDecision.Deny);
    }

    [Fact]
    public async Task EvaluateAsync_Should_Deny_By_Default_When_No_Rules_Match()
    {
        // Arrange
        var context = new AuthorizationEvaluationContext(
            "user-001",
            "resource-001",
            "operation-001",
            Array.Empty<string>());
        var resources = Array.Empty<Resource>();

        // Act
        var result = await _engine.EvaluateAsync(context, Array.Empty<AuthorizationRule>(), resources);

        // Assert
        result.IsAuthorized.Should().BeFalse();
        result.Decision.Should().Be(AuthorizationRuleDecision.Deny);
        result.AppliedRules.Should().BeEmpty();
    }

    [Fact]
    public async Task EvaluateAsync_Should_Resolve_Inheritance_From_Parent()
    {
        // Arrange
        var user = User.Create("user-001", "tenant-001", UserType.EndUser);
        var root = Resource.CreateRoot("resource-root", "app-001", "Dashboard", "dashboard");
        var child = Resource.CreateChild("resource-child", "app-001", "Users", "users", root.Id);
        var resources = new[] { root, child };
        var operation = Operation.Create("operation-read", "app-001", "read", "Read");
        var inheritedRule = AuthorizationRule.CreateForUser(
            "rule-child",
            user,
            child,
            operation,
            AuthorizationRuleDecision.Inherit);
        var parentRule = AuthorizationRule.CreateForUser(
            "rule-parent",
            user,
            root,
            operation,
            AuthorizationRuleDecision.Deny);
        var context = new AuthorizationEvaluationContext(
            "user-001",
            "resource-child",
            "operation-read",
            Array.Empty<string>());

        // Act
        var result = await _engine.EvaluateAsync(context, new[] { inheritedRule, parentRule }, resources);

        // Assert
        result.IsAuthorized.Should().BeFalse();
        result.Decision.Should().Be(AuthorizationRuleDecision.Deny);
        result.ResolvedResourceId.Should().Be("resource-root");
        result.AppliedRules.Should().Contain(parentRule);
    }

    [Fact]
    public async Task EvaluateAsync_Should_Resolve_Inheritance_Through_Multiple_Levels()
    {
        // Arrange
        var user = User.Create("user-001", "tenant-001", UserType.EndUser);
        var root = Resource.CreateRoot("resource-root", "app-001", "Dashboard", "dashboard");
        var middle = Resource.CreateChild("resource-middle", "app-001", "Admin", "admin", root.Id);
        var child = Resource.CreateChild("resource-child", "app-001", "Users", "users", middle.Id);
        var resources = new[] { root, middle, child };
        var operation = Operation.Create("operation-read", "app-001", "read", "Read");
        var childRule = AuthorizationRule.CreateForUser("rule-child", user, child, operation, AuthorizationRuleDecision.Inherit);
        var middleRule = AuthorizationRule.CreateForUser("rule-middle", user, middle, operation, AuthorizationRuleDecision.Inherit);
        var rootRule = AuthorizationRule.CreateForUser("rule-root", user, root, operation, AuthorizationRuleDecision.Allow);
        var context = new AuthorizationEvaluationContext(
            "user-001",
            "resource-child",
            "operation-read",
            Array.Empty<string>());

        // Act
        var result = await _engine.EvaluateAsync(context, new[] { childRule, middleRule, rootRule }, resources);

        // Assert
        result.IsAuthorized.Should().BeTrue();
        result.Decision.Should().Be(AuthorizationRuleDecision.Allow);
        result.ResolvedResourceId.Should().Be("resource-root");
        result.AppliedRules.Should().Contain(rootRule);
    }

    [Fact]
    public async Task EvaluateAsync_Should_Ignore_Rules_For_Different_Operation()
    {
        // Arrange
        var user = User.Create("user-001", "tenant-001", UserType.EndUser);
        var resource = Resource.CreateRoot("resource-001", "app-001", "Dashboard", "dashboard");
        var resources = new[] { resource };
        var readOp = Operation.Create("operation-read", "app-001", "read", "Read");
        var writeOp = Operation.Create("operation-write", "app-001", "write", "Write");
        var inheritedRule = AuthorizationRule.CreateForUser(
            "rule-child",
            user,
            resource,
            readOp,
            AuthorizationRuleDecision.Inherit);
        var parentRule = AuthorizationRule.CreateForUser(
            "rule-parent",
            user,
            resource,
            writeOp,
            AuthorizationRuleDecision.Allow);
        var context = new AuthorizationEvaluationContext(
            "user-001",
            "resource-001",
            "operation-read",
            Array.Empty<string>());

        // Act
        var result = await _engine.EvaluateAsync(context, new[] { inheritedRule, parentRule }, resources);

        // Assert
        result.IsAuthorized.Should().BeFalse();
        result.Decision.Should().Be(AuthorizationRuleDecision.Deny);
    }

    [Fact]
    public async Task EvaluateAsync_Should_Respect_RoleBased_Rules()
    {
        // Arrange
        var user = User.Create("user-001", "tenant-001", UserType.EndUser);
        var role = Role.Create("role-001", "tenant-001", "Operators");
        var resource = Resource.CreateRoot("resource-001", "app-001", "Dashboard", "dashboard");
        var resources = new[] { resource };
        var operation = Operation.Create("operation-001", "app-001", "read", "Read");
        var rule = AuthorizationRule.CreateForRole(
            "rule-001",
            role,
            resource,
            operation,
            AuthorizationRuleDecision.Allow);
        var context = new AuthorizationEvaluationContext(
            "user-001",
            "resource-001",
            "operation-001",
            new[] { "role-001" });

        // Act
        var result = await _engine.EvaluateAsync(context, new[] { rule }, resources);

        // Assert
        result.IsAuthorized.Should().BeTrue();
        result.Decision.Should().Be(AuthorizationRuleDecision.Allow);
    }

    [Fact]
    public async Task EvaluateAsync_Should_Respect_UserAndRole_Rule()
    {
        // Arrange
        var user = User.Create("user-001", "tenant-001", UserType.EndUser);
        var role = Role.Create("role-001", "tenant-001", "Operators");
        var resource = Resource.CreateRoot("resource-001", "app-001", "Dashboard", "dashboard");
        var resources = new[] { resource };
        var operation = Operation.Create("operation-001", "app-001", "read", "Read");
        var rule = AuthorizationRule.CreateForUserAndRole(
            "rule-001",
            user,
            role,
            resource,
            operation,
            AuthorizationRuleDecision.Allow);
        var context = new AuthorizationEvaluationContext(
            "user-001",
            "resource-001",
            "operation-001",
            new[] { "role-001" });

        // Act
        var result = await _engine.EvaluateAsync(context, new[] { rule }, resources);

        // Assert
        result.IsAuthorized.Should().BeTrue();
        result.Decision.Should().Be(AuthorizationRuleDecision.Allow);
    }

    [Fact]
    public async Task EvaluateAsync_Should_Reject_UserAndRole_Rule_When_Only_User_Matches()
    {
        // Arrange
        var user = User.Create("user-001", "tenant-001", UserType.EndUser);
        var role = Role.Create("role-001", "tenant-001", "Operators");
        var resource = Resource.CreateRoot("resource-001", "app-001", "Dashboard", "dashboard");
        var resources = new[] { resource };
        var operation = Operation.Create("operation-001", "app-001", "read", "Read");
        var rule = AuthorizationRule.CreateForUserAndRole(
            "rule-001",
            user,
            role,
            resource,
            operation,
            AuthorizationRuleDecision.Allow);
        var context = new AuthorizationEvaluationContext(
            "user-001",
            "resource-001",
            "operation-001",
            Array.Empty<string>());

        // Act
        var result = await _engine.EvaluateAsync(context, new[] { rule }, resources);

        // Assert
        result.IsAuthorized.Should().BeFalse();
    }

    [Fact]
    public async Task EvaluateAsync_Should_Reject_Inactive_Rules()
    {
        // Arrange
        var user = User.Create("user-001", "tenant-001", UserType.EndUser);
        var resource = Resource.CreateRoot("resource-001", "app-001", "Dashboard", "dashboard");
        var resources = new[] { resource };
        var operation = Operation.Create("operation-001", "app-001", "read", "Read");
        var rule = AuthorizationRule.CreateForUser(
            "rule-001",
            user,
            resource,
            operation,
            AuthorizationRuleDecision.Allow);
        rule.Deactivate();
        var context = new AuthorizationEvaluationContext(
            "user-001",
            "resource-001",
            "operation-001",
            Array.Empty<string>());

        // Act
        var result = await _engine.EvaluateAsync(context, new[] { rule }, resources);

        // Assert
        result.IsAuthorized.Should().BeFalse();
        result.AppliedRules.Should().BeEmpty();
    }
}