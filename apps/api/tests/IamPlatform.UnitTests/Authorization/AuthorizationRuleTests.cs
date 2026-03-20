using FluentAssertions;
using IamPlatform.Domain.Authorization;
using IamPlatform.Domain.Tenants;
using Xunit;

namespace IamPlatform.UnitTests.Authorization;

public sealed class AuthorizationRuleTests
{
    [Fact]
    public void CreateForTenantUser_Should_Set_TenantUser_Target()
    {
        var tenantUser = TenantUser.Create("tenant-user-001", "tenant-001", TenantUserType.EndUser);
        var resource = Resource.CreateRoot("resource-001", "app-001", "Dashboard", "dashboard");
        var operation = Operation.Create("operation-001", "app-001", "read", "Read");

        var rule = AuthorizationRule.CreateForTenantUser(
            "rule-001",
            tenantUser,
            resource,
            operation,
            AuthorizationRuleDecision.Allow);

        rule.Id.Should().Be("rule-001");
        rule.ResourceId.Should().Be("resource-001");
        rule.OperationId.Should().Be("operation-001");
        rule.TenantUserId.Should().Be("tenant-user-001");
        rule.RoleId.Should().BeNull();
        rule.Decision.Should().Be(AuthorizationRuleDecision.Allow);
        rule.AppliesToTenantUserOnly.Should().BeTrue();
        rule.IsActive.Should().BeTrue();
    }

    [Fact]
    public void CreateForRole_Should_Set_Role_Target()
    {
        var role = Role.Create("role-001", "tenant-001", "Operators");
        var resource = Resource.CreateRoot("resource-001", "app-001", "Dashboard", "dashboard");
        var operation = Operation.Create("operation-001", "app-001", "read", "Read");

        var rule = AuthorizationRule.CreateForRole(
            "rule-001",
            role,
            resource,
            operation,
            AuthorizationRuleDecision.Deny);

        rule.TenantUserId.Should().BeNull();
        rule.RoleId.Should().Be("role-001");
        rule.Decision.Should().Be(AuthorizationRuleDecision.Deny);
        rule.AppliesToRoleOnly.Should().BeTrue();
    }

    [Fact]
    public void CreateForTenantUserAndRole_Should_Set_Both_Targets()
    {
        var tenantUser = TenantUser.Create("tenant-user-001", "tenant-001", TenantUserType.EndUser);
        var role = Role.Create("role-001", "tenant-001", "Operators");
        var resource = Resource.CreateRoot("resource-001", "app-001", "Dashboard", "dashboard");
        var operation = Operation.Create("operation-001", "app-001", "read", "Read");

        var rule = AuthorizationRule.CreateForTenantUserAndRole(
            "rule-001",
            tenantUser,
            role,
            resource,
            operation,
            AuthorizationRuleDecision.Inherit);

        rule.TenantUserId.Should().Be("tenant-user-001");
        rule.RoleId.Should().Be("role-001");
        rule.Decision.Should().Be(AuthorizationRuleDecision.Inherit);
        rule.AppliesToTenantUserAndRole.Should().BeTrue();
    }

    [Fact]
    public void CreateForTenantUserAndRole_Should_Reject_Different_Tenants()
    {
        var tenantUser = TenantUser.Create("tenant-user-001", "tenant-001", TenantUserType.EndUser);
        var role = Role.Create("role-001", "tenant-002", "Operators");
        var resource = Resource.CreateRoot("resource-001", "app-001", "Dashboard", "dashboard");
        var operation = Operation.Create("operation-001", "app-001", "read", "Read");

        var act = () => AuthorizationRule.CreateForTenantUserAndRole(
            "rule-001",
            tenantUser,
            role,
            resource,
            operation,
            AuthorizationRuleDecision.Allow);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Tenant user and role must belong to the same tenant.");
    }

    [Fact]
    public void Create_Should_Reject_When_Resource_And_Operation_Belong_To_Different_Applications()
    {
        var tenantUser = TenantUser.Create("tenant-user-001", "tenant-001", TenantUserType.EndUser);
        var resource = Resource.CreateRoot("resource-001", "app-001", "Dashboard", "dashboard");
        var operation = Operation.Create("operation-001", "app-002", "read", "Read");

        var act = () => AuthorizationRule.CreateForTenantUser(
            "rule-001",
            tenantUser,
            resource,
            operation,
            AuthorizationRuleDecision.Allow);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Resource and operation must belong to the same application.");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_Should_Reject_Invalid_Rule_Id(string invalidId)
    {
        var tenantUser = TenantUser.Create("tenant-user-001", "tenant-001", TenantUserType.EndUser);
        var resource = Resource.CreateRoot("resource-001", "app-001", "Dashboard", "dashboard");
        var operation = Operation.Create("operation-001", "app-001", "read", "Read");

        var act = () => AuthorizationRule.CreateForTenantUser(
            invalidId,
            tenantUser,
            resource,
            operation,
            AuthorizationRuleDecision.Allow);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Authorization rule id is required.*");
    }

    [Fact]
    public void ChangeDecision_Should_Update_Decision()
    {
        var tenantUser = TenantUser.Create("tenant-user-001", "tenant-001", TenantUserType.EndUser);
        var resource = Resource.CreateRoot("resource-001", "app-001", "Dashboard", "dashboard");
        var operation = Operation.Create("operation-001", "app-001", "read", "Read");
        var rule = AuthorizationRule.CreateForTenantUser(
            "rule-001",
            tenantUser,
            resource,
            operation,
            AuthorizationRuleDecision.Allow);

        rule.ChangeDecision(AuthorizationRuleDecision.Deny);

        rule.Decision.Should().Be(AuthorizationRuleDecision.Deny);
    }

    [Fact]
    public void Deactivate_Should_Set_Inactive_Status()
    {
        var tenantUser = TenantUser.Create("tenant-user-001", "tenant-001", TenantUserType.EndUser);
        var resource = Resource.CreateRoot("resource-001", "app-001", "Dashboard", "dashboard");
        var operation = Operation.Create("operation-001", "app-001", "read", "Read");
        var rule = AuthorizationRule.CreateForTenantUser(
            "rule-001",
            tenantUser,
            resource,
            operation,
            AuthorizationRuleDecision.Allow);

        rule.Deactivate();

        rule.IsActive.Should().BeFalse();
    }
}
