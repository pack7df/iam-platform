using FluentAssertions;
using IamPlatform.Domain.Authorization;
using IamPlatform.Domain.Tenants;
using IamPlatform.Application.Authorization;
using Xunit;

namespace IamPlatform.UnitTests.Authorization;

public sealed class AuthorizationServiceTests
{
    [Fact]
    public async Task EvaluateAsync_Should_Allow_When_User_Has_Allow_Rule()
    {
        // Arrange
        var userId = "user-001";
        var tenantId = "tenant-001";
        var resourceId = "resource-001";
        var operationId = "op-001";
        var roleIds = new string[0];
        var applicationId = "app-001";

        var user = User.Create(userId, tenantId, UserType.EndUser);
        var resource = Resource.CreateRoot(resourceId, applicationId, "Test Resource", "test");
        var operation = Operation.Create(operationId, applicationId, "read", "Read");
        var rule = AuthorizationRule.CreateForUser(
            "rule-001",
            user,
            resource,
            operation,
            AuthorizationRuleDecision.Allow);

        var resourceRepo = new FakeResourceRepository(new[] { resource });
        var ruleRepo = new FakeAuthorizationRuleRepository(new[] { rule });
        var engine = new AuthorizationEngine();

        var service = new AuthorizationService(engine, resourceRepo, ruleRepo);

        // Act
        var result = await service.EvaluateAsync(userId, resourceId, operationId, roleIds);

        // Assert
        result.IsAuthorized.Should().BeTrue();
        result.Decision.Should().Be(AuthorizationRuleDecision.Allow);
        result.AppliedRules.Should().ContainSingle();
    }

    [Fact]
    public async Task EvaluateAsync_Should_Deny_When_No_Rules_Match()
    {
        // Arrange
        var userId = "user-001";
        var resourceId = "resource-001";
        var operationId = "op-001";
        var roleIds = new string[0];
        var applicationId = "app-001";

        var resource = Resource.CreateRoot(resourceId, applicationId, "Test Resource", "test");
        var resourceRepo = new FakeResourceRepository(new[] { resource });
        var ruleRepo = new FakeAuthorizationRuleRepository(Array.Empty<AuthorizationRule>());
        var engine = new AuthorizationEngine();

        var service = new AuthorizationService(engine, resourceRepo, ruleRepo);

        // Act
        var result = await service.EvaluateAsync(userId, resourceId, operationId, roleIds);

        // Assert
        result.IsAuthorized.Should().BeFalse();
        result.Decision.Should().Be(AuthorizationRuleDecision.Deny);
        result.AppliedRules.Should().BeEmpty();
    }

    [Fact]
    public async Task EvaluateAsync_Should_Combine_User_And_Role_Rules_With_Deny_Precedence()
    {
        // Arrange
        var userId = "user-001";
        var tenantId = "tenant-001";
        var resourceId = "resource-001";
        var operationId = "op-001";
        var roleIds = new[] { "role-001" };
        var applicationId = "app-001";

        var user = User.Create(userId, tenantId, UserType.EndUser);
        var role = Role.Create("role-001", tenantId, "Operators");
        var resource = Resource.CreateRoot(resourceId, applicationId, "Test Resource", "test");
        var operation = Operation.Create(operationId, applicationId, "read", "Read");

        var userAllowRule = AuthorizationRule.CreateForUser(
            "rule-user",
            user,
            resource,
            operation,
            AuthorizationRuleDecision.Allow);
        var roleDenyRule = AuthorizationRule.CreateForRole(
            "rule-role",
            role,
            resource,
            operation,
            AuthorizationRuleDecision.Deny);

        var resourceRepo = new FakeResourceRepository(new[] { resource });
        var ruleRepo = new FakeAuthorizationRuleRepository(new[] { userAllowRule, roleDenyRule });
        var engine = new AuthorizationEngine();

        var service = new AuthorizationService(engine, resourceRepo, ruleRepo);

        // Act
        var result = await service.EvaluateAsync(userId, resourceId, operationId, roleIds);

        // Assert
        result.IsAuthorized.Should().BeFalse(); // Deny takes precedence
        result.Decision.Should().Be(AuthorizationRuleDecision.Deny);
    }

    [Fact]
    public async Task EvaluateAsync_Should_Resolve_Inheritance_Through_Service()
    {
        // Arrange
        var userId = "user-001";
        var tenantId = "tenant-001";
        var applicationId = "app-001";

        var user = User.Create(userId, tenantId, UserType.EndUser);
        var root = Resource.CreateRoot("resource-root", applicationId, "Root", "root");
        var child = Resource.CreateChild("resource-child", applicationId, "Child", "child", root.Id);
        var operation = Operation.Create("op-read", applicationId, "read", "Read");

        var inheritRule = AuthorizationRule.CreateForUser(
            "rule-inherit",
            user,
            child,
            operation,
            AuthorizationRuleDecision.Inherit);
        var parentAllowRule = AuthorizationRule.CreateForUser(
            "rule-parent",
            user,
            root,
            operation,
            AuthorizationRuleDecision.Allow);

        var resourceRepo = new FakeResourceRepository(new[] { root, child });
        var ruleRepo = new FakeAuthorizationRuleRepository(new[] { inheritRule, parentAllowRule });
        var engine = new AuthorizationEngine();

        var service = new AuthorizationService(engine, resourceRepo, ruleRepo);

        // Act
        var result = await service.EvaluateAsync(userId, "resource-child", "op-read", Array.Empty<string>());

        // Assert
        result.IsAuthorized.Should().BeTrue();
        result.Decision.Should().Be(AuthorizationRuleDecision.Allow);
        result.ResolvedResourceId.Should().Be("resource-root");
        result.AppliedRules.Should().Contain(parentAllowRule);
    }

    [Fact]
    public async Task EvaluateAsync_Should_Deny_When_Resource_Not_Found()
    {
        // Arrange
        var userId = "user-001";
        var resourceId = "non-existent";
        var operationId = "op-001";
        var roleIds = Array.Empty<string>();

        var resourceRepo = new FakeResourceRepository(Array.Empty<Resource>());
        var ruleRepo = new FakeAuthorizationRuleRepository(Array.Empty<AuthorizationRule>());
        var engine = new AuthorizationEngine();

        var service = new AuthorizationService(engine, resourceRepo, ruleRepo);

        // Act
        var result = await service.EvaluateAsync(userId, resourceId, operationId, roleIds);

        // Assert
        result.IsAuthorized.Should().BeFalse();
        result.Decision.Should().Be(AuthorizationRuleDecision.Deny);
        result.AppliedRules.Should().BeEmpty();
    }

    private sealed class FakeResourceRepository : IResourceRepository
    {
        private readonly List<Resource> _resources;

        public FakeResourceRepository(IEnumerable<Resource> resources)
        {
            _resources = resources.ToList();
        }

        public Task<IReadOnlyCollection<Resource>> GetAllForApplicationAsync(string applicationId, CancellationToken cancellationToken = default)
        {
            var result = _resources.Where(r => r.ApplicationId == applicationId).ToList();
            return Task.FromResult<IReadOnlyCollection<Resource>>(result);
        }

        public Task<Resource?> GetByIdAsync(string resourceId, CancellationToken cancellationToken = default)
        {
            var result = _resources.FirstOrDefault(r => r.Id == resourceId);
            return Task.FromResult<Resource?>(result);
        }
    }

    private sealed class FakeAuthorizationRuleRepository : IAuthorizationRuleRepository
    {
        private readonly List<AuthorizationRule> _rules;

        public FakeAuthorizationRuleRepository(IEnumerable<AuthorizationRule> rules)
        {
            _rules = rules.ToList();
        }

        public Task<IReadOnlyCollection<AuthorizationRule>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            var result = _rules.Where(r => r.UserId == userId).ToList();
            return Task.FromResult<IReadOnlyCollection<AuthorizationRule>>(result);
        }

        public Task<IReadOnlyCollection<AuthorizationRule>> GetByRoleIdsAsync(IEnumerable<string> roleIds, CancellationToken cancellationToken = default)
        {
            var result = _rules.Where(r => r.RoleId != null && roleIds.Contains(r.RoleId)).ToList();
            return Task.FromResult<IReadOnlyCollection<AuthorizationRule>>(result);
        }
    }
}