using FluentAssertions;
using IamPlatform.Domain.Authorization;
using IamPlatform.Domain.Tenants;
using Xunit;

namespace IamPlatform.UnitTests.Authorization;

public sealed class AuthorizationRuleInheritanceResolverTests
{
    private readonly AuthorizationRuleInheritanceResolver _resolver = new();

    [Fact]
    public void Resolve_Should_Return_Explicit_Decision_Immediately()
    {
        var resource = Resource.CreateRoot("resource-root", "app-001", "Dashboard", "dashboard");
        var operation = Operation.Create("operation-read", "app-001", "read", "Read");
        var tenantUser = TenantUser.Create("tenant-user-001", "tenant-001", TenantUserType.EndUser);
        var rule = AuthorizationRule.CreateForTenantUser(
            "rule-001",
            tenantUser,
            resource,
            operation,
            AuthorizationRuleDecision.Allow);

        var result = _resolver.Resolve(rule, new[] { rule }, new[] { resource });

        result.IsResolved.Should().BeTrue();
        result.Decision.Should().Be(AuthorizationRuleDecision.Allow);
        result.ResolvedResourceId.Should().Be("resource-root");
    }

    [Fact]
    public void Resolve_Should_Use_Parent_Decision_For_Same_Operation_And_Same_Subject()
    {
        var root = Resource.CreateRoot("resource-root", "app-001", "Dashboard", "dashboard");
        var child = Resource.CreateChild("resource-child", "app-001", "Users", "users", root.Id);
        var operation = Operation.Create("operation-read", "app-001", "read", "Read");
        var tenantUser = TenantUser.Create("tenant-user-001", "tenant-001", TenantUserType.EndUser);
        var inheritedRule = AuthorizationRule.CreateForTenantUser(
            "rule-child",
            tenantUser,
            child,
            operation,
            AuthorizationRuleDecision.Inherit);
        var parentRule = AuthorizationRule.CreateForTenantUser(
            "rule-parent",
            tenantUser,
            root,
            operation,
            AuthorizationRuleDecision.Deny);

        var result = _resolver.Resolve(inheritedRule, new[] { inheritedRule, parentRule }, new[] { root, child });

        result.IsResolved.Should().BeTrue();
        result.Decision.Should().Be(AuthorizationRuleDecision.Deny);
        result.ResolvedResourceId.Should().Be("resource-root");
    }

    [Fact]
    public void Resolve_Should_Continue_Up_The_Tree_When_Parent_Is_Also_Inherited()
    {
        var root = Resource.CreateRoot("resource-root", "app-001", "Dashboard", "dashboard");
        var middle = Resource.CreateChild("resource-middle", "app-001", "Admin", "admin", root.Id);
        var child = Resource.CreateChild("resource-child", "app-001", "Users", "users", middle.Id);
        var operation = Operation.Create("operation-read", "app-001", "read", "Read");
        var tenantUser = TenantUser.Create("tenant-user-001", "tenant-001", TenantUserType.EndUser);
        var childRule = AuthorizationRule.CreateForTenantUser("rule-child", tenantUser, child, operation, AuthorizationRuleDecision.Inherit);
        var middleRule = AuthorizationRule.CreateForTenantUser("rule-middle", tenantUser, middle, operation, AuthorizationRuleDecision.Inherit);
        var rootRule = AuthorizationRule.CreateForTenantUser("rule-root", tenantUser, root, operation, AuthorizationRuleDecision.Allow);

        var result = _resolver.Resolve(childRule, new[] { childRule, middleRule, rootRule }, new[] { root, middle, child });

        result.IsResolved.Should().BeTrue();
        result.Decision.Should().Be(AuthorizationRuleDecision.Allow);
        result.ResolvedResourceId.Should().Be("resource-root");
    }

    [Fact]
    public void Resolve_Should_Ignore_Parent_Rule_For_Different_Operation()
    {
        var root = Resource.CreateRoot("resource-root", "app-001", "Dashboard", "dashboard");
        var child = Resource.CreateChild("resource-child", "app-001", "Users", "users", root.Id);
        var readOperation = Operation.Create("operation-read", "app-001", "read", "Read");
        var writeOperation = Operation.Create("operation-write", "app-001", "write", "Write");
        var tenantUser = TenantUser.Create("tenant-user-001", "tenant-001", TenantUserType.EndUser);
        var inheritedRule = AuthorizationRule.CreateForTenantUser("rule-child", tenantUser, child, readOperation, AuthorizationRuleDecision.Inherit);
        var parentRule = AuthorizationRule.CreateForTenantUser("rule-parent", tenantUser, root, writeOperation, AuthorizationRuleDecision.Allow);

        var result = _resolver.Resolve(inheritedRule, new[] { inheritedRule, parentRule }, new[] { root, child });

        result.IsResolved.Should().BeFalse();
        result.Decision.Should().BeNull();
    }

    [Fact]
    public void Resolve_Should_Ignore_Parent_Rule_For_Different_Subject()
    {
        var root = Resource.CreateRoot("resource-root", "app-001", "Dashboard", "dashboard");
        var child = Resource.CreateChild("resource-child", "app-001", "Users", "users", root.Id);
        var operation = Operation.Create("operation-read", "app-001", "read", "Read");
        var tenantUser = TenantUser.Create("tenant-user-001", "tenant-001", TenantUserType.EndUser);
        var anotherTenantUser = TenantUser.Create("tenant-user-002", "tenant-001", TenantUserType.EndUser);
        var inheritedRule = AuthorizationRule.CreateForTenantUser("rule-child", tenantUser, child, operation, AuthorizationRuleDecision.Inherit);
        var parentRule = AuthorizationRule.CreateForTenantUser("rule-parent", anotherTenantUser, root, operation, AuthorizationRuleDecision.Allow);

        var result = _resolver.Resolve(inheritedRule, new[] { inheritedRule, parentRule }, new[] { root, child });

        result.IsResolved.Should().BeFalse();
    }

    [Fact]
    public void Resolve_Should_Return_Unresolved_When_Chain_Reaches_Root_Without_Explicit_Decision()
    {
        var root = Resource.CreateRoot("resource-root", "app-001", "Dashboard", "dashboard");
        var child = Resource.CreateChild("resource-child", "app-001", "Users", "users", root.Id);
        var operation = Operation.Create("operation-read", "app-001", "read", "Read");
        var tenantUser = TenantUser.Create("tenant-user-001", "tenant-001", TenantUserType.EndUser);
        var inheritedRule = AuthorizationRule.CreateForTenantUser("rule-child", tenantUser, child, operation, AuthorizationRuleDecision.Inherit);
        var rootInheritedRule = AuthorizationRule.CreateForTenantUser("rule-root", tenantUser, root, operation, AuthorizationRuleDecision.Inherit);

        var result = _resolver.Resolve(inheritedRule, new[] { inheritedRule, rootInheritedRule }, new[] { root, child });

        result.IsResolved.Should().BeFalse();
        result.Decision.Should().BeNull();
        result.ResolvedResourceId.Should().BeNull();
    }

    [Fact]
    public void Resolve_Should_Return_Unresolved_When_Root_Has_No_Matching_Rule()
    {
        var root = Resource.CreateRoot("resource-root", "app-001", "Dashboard", "dashboard");
        var child = Resource.CreateChild("resource-child", "app-001", "Users", "users", root.Id);
        var operation = Operation.Create("operation-read", "app-001", "read", "Read");
        var tenantUser = TenantUser.Create("tenant-user-001", "tenant-001", TenantUserType.EndUser);
        var inheritedRule = AuthorizationRule.CreateForTenantUser("rule-child", tenantUser, child, operation, AuthorizationRuleDecision.Inherit);

        var result = _resolver.Resolve(inheritedRule, new[] { inheritedRule }, new[] { root, child });

        result.IsResolved.Should().BeFalse();
    }
}
