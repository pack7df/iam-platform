using FluentAssertions;
using IamPlatform.Domain.Authorization;
using IamPlatform.Domain.Tenants;
using Xunit;

namespace IamPlatform.UnitTests.Authorization;

public sealed class AuthorizationEngineTests
{
    [Fact]
    public async Task EvaluateAsync_Should_Allow_When_Single_Allow_Rule_Matches()
    {
        // Arrange
        var userId = "user-001";
        var resourceId = "resource-001";
        var operationId = "operation-001";

        var user = User.Create(userId, "tenant-001", UserType.EndUser);
        var resource = Resource.CreateRoot(resourceId, "app-001", "Dashboard", "dashboard");
        var operation = Operation.Create(operationId, "app-001", "read", "Read");
        var rule = AuthorizationRule.CreateForUser(
            "rule-001",
            user,
            resource,
            operation,
            AuthorizationRuleDecision.Allow);

        var resourceRepo = new FakeResourceRepository(new[] { resource });
        var userRoleAssignmentRepo = new FakeUserRoleAssignmentRepository(Array.Empty<string>());
        var ruleRepo = new FakeAuthorizationRuleRepository(new[] { rule }, userRoleAssignmentRepo);
        var engine = new AuthorizationEngine(ruleRepo, resourceRepo);

        // Act
        var result = await engine.EvaluateAsync(userId, resourceId, operationId);

        // Assert
        result.IsAuthorized.Should().BeTrue();
        result.Decision.Should().Be(AuthorizationRuleDecision.Allow);
        result.ResolvedResourceId.Should().Be(resourceId);
        result.UserId.Should().Be(userId);
        result.AppliedRules.Should().ContainSingle();
    }

    [Fact]
    public async Task EvaluateAsync_Should_Deny_When_Single_Deny_Rule_Matches()
    {
        // Arrange
        var userId = "user-001";
        var resourceId = "resource-001";
        var operationId = "operation-001";

        var user = User.Create(userId, "tenant-001", UserType.EndUser);
        var resource = Resource.CreateRoot(resourceId, "app-001", "Dashboard", "dashboard");
        var operation = Operation.Create(operationId, "app-001", "read", "Read");
        var rule = AuthorizationRule.CreateForUser(
            "rule-001",
            user,
            resource,
            operation,
            AuthorizationRuleDecision.Deny);

        var resourceRepo = new FakeResourceRepository(new[] { resource });
        var userRoleAssignmentRepo = new FakeUserRoleAssignmentRepository(Array.Empty<string>());
        var ruleRepo = new FakeAuthorizationRuleRepository(new[] { rule }, userRoleAssignmentRepo);
        var engine = new AuthorizationEngine(ruleRepo, resourceRepo);

        // Act
        var result = await engine.EvaluateAsync(userId, resourceId, operationId);

        // Assert
        result.IsAuthorized.Should().BeFalse();
        result.Decision.Should().Be(AuthorizationRuleDecision.Deny);
        result.ResolvedResourceId.Should().Be(resourceId);
        result.UserId.Should().Be(userId);
    }

    [Fact]
    public async Task EvaluateAsync_Should_Allow_When_Multiple_Allow_Rules_All_Allow()
    {
        // Arrange
        var userId = "user-001";
        var resourceId = "resource-001";
        var operationId = "operation-001";

        var user = User.Create(userId, "tenant-001", UserType.EndUser);
        var resource = Resource.CreateRoot(resourceId, "app-001", "Dashboard", "dashboard");
        var operation = Operation.Create(operationId, "app-001", "read", "Read");
        var rule1 = AuthorizationRule.CreateForUser("rule-001", user, resource, operation, AuthorizationRuleDecision.Allow);
        var rule2 = AuthorizationRule.CreateForUser("rule-002", user, resource, operation, AuthorizationRuleDecision.Allow);

        var resourceRepo = new FakeResourceRepository(new[] { resource });
        var userRoleAssignmentRepo = new FakeUserRoleAssignmentRepository(Array.Empty<string>());
        var ruleRepo = new FakeAuthorizationRuleRepository(new[] { rule1, rule2 }, userRoleAssignmentRepo);
        var engine = new AuthorizationEngine(ruleRepo, resourceRepo);

        // Act
        var result = await engine.EvaluateAsync(userId, resourceId, operationId);

        // Assert
        result.IsAuthorized.Should().BeTrue();
        result.Decision.Should().Be(AuthorizationRuleDecision.Allow);
        result.UserId.Should().Be(userId);
    }

    [Fact]
    public async Task EvaluateAsync_Should_Deny_When_Any_Deny_Rule_Among_Allows()
    {
        // Arrange
        var userId = "user-001";
        var resourceId = "resource-001";
        var operationId = "operation-001";

        var user = User.Create(userId, "tenant-001", UserType.EndUser);
        var resource = Resource.CreateRoot(resourceId, "app-001", "Dashboard", "dashboard");
        var operation = Operation.Create(operationId, "app-001", "read", "Read");
        var rule1 = AuthorizationRule.CreateForUser("rule-001", user, resource, operation, AuthorizationRuleDecision.Allow);
        var rule2 = AuthorizationRule.CreateForUser("rule-002", user, resource, operation, AuthorizationRuleDecision.Deny);
        var rule3 = AuthorizationRule.CreateForUser("rule-003", user, resource, operation, AuthorizationRuleDecision.Allow);

        var resourceRepo = new FakeResourceRepository(new[] { resource });
        var userRoleAssignmentRepo = new FakeUserRoleAssignmentRepository(Array.Empty<string>());
        var ruleRepo = new FakeAuthorizationRuleRepository(new[] { rule1, rule2, rule3 }, userRoleAssignmentRepo);
        var engine = new AuthorizationEngine(ruleRepo, resourceRepo);

        // Act
        var result = await engine.EvaluateAsync(userId, resourceId, operationId);

        // Assert
        result.IsAuthorized.Should().BeFalse();
        result.Decision.Should().Be(AuthorizationRuleDecision.Deny);
        result.UserId.Should().Be(userId);
    }

    [Fact]
    public async Task EvaluateAsync_Should_Deny_By_Default_When_No_Rules_Match()
    {
        // Arrange
        var userId = "user-001";
        var resourceId = "resource-001";
        var operationId = "operation-001";

        var resource = Resource.CreateRoot(resourceId, "app-001", "Dashboard", "dashboard");
        var resourceRepo = new FakeResourceRepository(new[] { resource });
        var userRoleAssignmentRepo = new FakeUserRoleAssignmentRepository(Array.Empty<string>());
        var ruleRepo = new FakeAuthorizationRuleRepository(Array.Empty<AuthorizationRule>(), userRoleAssignmentRepo);
        var engine = new AuthorizationEngine(ruleRepo, resourceRepo);

        // Act
        var result = await engine.EvaluateAsync(userId, resourceId, operationId);

        // Assert
        result.IsAuthorized.Should().BeFalse();
        result.Decision.Should().Be(AuthorizationRuleDecision.Deny);
        result.UserId.Should().Be(userId);
        result.ResolvedResourceId.Should().Be(resourceId);
        result.AppliedRules.Should().BeEmpty();
    }

    [Fact]
    public async Task EvaluateAsync_Should_Resolve_Inheritance_From_Parent()
    {
        // Arrange
        var userId = "user-001";
        var resourceId = "resource-child";
        var operationId = "operation-read";

        var user = User.Create(userId, "tenant-001", UserType.EndUser);
        var root = Resource.CreateRoot("resource-root", "app-001", "Dashboard", "dashboard");
        var child = Resource.CreateChild("resource-child", "app-001", "Users", "users", root.Id);
        var operation = Operation.Create(operationId, "app-001", "read", "Read");
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

        var resourceRepo = new FakeResourceRepository(new[] { root, child });
        var userRoleAssignmentRepo = new FakeUserRoleAssignmentRepository(Array.Empty<string>());
        var ruleRepo = new FakeAuthorizationRuleRepository(new[] { inheritedRule, parentRule }, userRoleAssignmentRepo);
        var engine = new AuthorizationEngine(ruleRepo, resourceRepo);

        // Act
        var result = await engine.EvaluateAsync(userId, resourceId, operationId);

        // Assert
        result.IsAuthorized.Should().BeFalse();
        result.Decision.Should().Be(AuthorizationRuleDecision.Deny);
        result.ResolvedResourceId.Should().Be("resource-root");
        result.UserId.Should().Be(userId);
        result.AppliedRules.Should().Contain(parentRule);
    }

    [Fact]
    public async Task EvaluateAsync_Should_Resolve_Inheritance_Through_Multiple_Levels()
    {
        // Arrange
        var userId = "user-001";
        var resourceId = "resource-child";
        var operationId = "operation-read";

        var user = User.Create(userId, "tenant-001", UserType.EndUser);
        var root = Resource.CreateRoot("resource-root", "app-001", "Dashboard", "dashboard");
        var middle = Resource.CreateChild("resource-middle", "app-001", "Admin", "admin", root.Id);
        var child = Resource.CreateChild("resource-child", "app-001", "Users", "users", middle.Id);
        var operation = Operation.Create(operationId, "app-001", "read", "Read");
        var childRule = AuthorizationRule.CreateForUser("rule-child", user, child, operation, AuthorizationRuleDecision.Inherit);
        var middleRule = AuthorizationRule.CreateForUser("rule-middle", user, middle, operation, AuthorizationRuleDecision.Inherit);
        var rootRule = AuthorizationRule.CreateForUser("rule-root", user, root, operation, AuthorizationRuleDecision.Allow);

        var resourceRepo = new FakeResourceRepository(new[] { root, middle, child });
        var userRoleAssignmentRepo = new FakeUserRoleAssignmentRepository(Array.Empty<string>());
        var ruleRepo = new FakeAuthorizationRuleRepository(new[] { childRule, middleRule, rootRule }, userRoleAssignmentRepo);
        var engine = new AuthorizationEngine(ruleRepo, resourceRepo);

        // Act
        var result = await engine.EvaluateAsync(userId, resourceId, operationId);

        // Assert
        result.IsAuthorized.Should().BeTrue();
        result.Decision.Should().Be(AuthorizationRuleDecision.Allow);
        result.ResolvedResourceId.Should().Be("resource-root");
        result.UserId.Should().Be(userId);
        result.AppliedRules.Should().Contain(rootRule);
    }

    [Fact]
    public async Task EvaluateAsync_Should_Ignore_Rules_For_Different_Operation()
    {
        // Arrange
        var userId = "user-001";
        var resourceId = "resource-001";
        var operationId = "operation-read";

        var user = User.Create(userId, "tenant-001", UserType.EndUser);
        var resource = Resource.CreateRoot(resourceId, "app-001", "Dashboard", "dashboard");
        var readOp = Operation.Create(operationId, "app-001", "read", "Read");
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

        var resourceRepo = new FakeResourceRepository(new[] { resource });
        var userRoleAssignmentRepo = new FakeUserRoleAssignmentRepository(Array.Empty<string>());
        var ruleRepo = new FakeAuthorizationRuleRepository(new[] { inheritedRule, parentRule }, userRoleAssignmentRepo);
        var engine = new AuthorizationEngine(ruleRepo, resourceRepo);

        // Act
        var result = await engine.EvaluateAsync(userId, resourceId, operationId);

        // Assert
        result.IsAuthorized.Should().BeFalse();
        result.Decision.Should().Be(AuthorizationRuleDecision.Deny);
        result.UserId.Should().Be(userId);
    }

    [Fact]
    public async Task EvaluateAsync_Should_Respect_RoleBased_Rules()
    {
        // Arrange
        var userId = "user-001";
        var resourceId = "resource-001";
        var operationId = "operation-001";
        var roleId = "role-001";

        var user = User.Create(userId, "tenant-001", UserType.EndUser);
        var role = Role.Create(roleId, "tenant-001", "Operators");
        var resource = Resource.CreateRoot(resourceId, "app-001", "Dashboard", "dashboard");
        var operation = Operation.Create(operationId, "app-001", "read", "Read");
        var rule = AuthorizationRule.CreateForRole(
            "rule-001",
            role,
            resource,
            operation,
            AuthorizationRuleDecision.Allow);

        var resourceRepo = new FakeResourceRepository(new[] { resource });
        var userRoleAssignmentRepo = new FakeUserRoleAssignmentRepository(new[] { roleId });
        var ruleRepo = new FakeAuthorizationRuleRepository(new[] { rule }, userRoleAssignmentRepo);
        var engine = new AuthorizationEngine(ruleRepo, resourceRepo);

        // Act
        var result = await engine.EvaluateAsync(userId, resourceId, operationId);

        // Assert
        result.IsAuthorized.Should().BeTrue();
        result.Decision.Should().Be(AuthorizationRuleDecision.Allow);
        result.UserId.Should().Be(userId);
    }

    [Fact]
    public async Task EvaluateAsync_Should_Respect_UserAndRole_Rule()
    {
        // Arrange
        var userId = "user-001";
        var resourceId = "resource-001";
        var operationId = "operation-001";
        var roleId = "role-001";

        var user = User.Create(userId, "tenant-001", UserType.EndUser);
        var role = Role.Create(roleId, "tenant-001", "Operators");
        var resource = Resource.CreateRoot(resourceId, "app-001", "Dashboard", "dashboard");
        var operation = Operation.Create(operationId, "app-001", "read", "Read");
        var rule = AuthorizationRule.CreateForUserAndRole(
            "rule-001",
            user,
            role,
            resource,
            operation,
            AuthorizationRuleDecision.Allow);

        var resourceRepo = new FakeResourceRepository(new[] { resource });
        var userRoleAssignmentRepo = new FakeUserRoleAssignmentRepository(new[] { roleId });
        var ruleRepo = new FakeAuthorizationRuleRepository(new[] { rule }, userRoleAssignmentRepo);
        var engine = new AuthorizationEngine(ruleRepo, resourceRepo);

        // Act
        var result = await engine.EvaluateAsync(userId, resourceId, operationId);

        // Assert
        result.IsAuthorized.Should().BeTrue();
        result.Decision.Should().Be(AuthorizationRuleDecision.Allow);
        result.UserId.Should().Be(userId);
    }

    [Fact]
    public async Task EvaluateAsync_Should_Reject_UserAndRole_Rule_When_Only_User_Matches()
    {
        // Arrange
        var userId = "user-001";
        var resourceId = "resource-001";
        var operationId = "operation-001";

        var user = User.Create(userId, "tenant-001", UserType.EndUser);
        var role = Role.Create("role-001", "tenant-001", "Operators");
        var resource = Resource.CreateRoot(resourceId, "app-001", "Dashboard", "dashboard");
        var operation = Operation.Create(operationId, "app-001", "read", "Read");
        var rule = AuthorizationRule.CreateForUserAndRole(
            "rule-001",
            user,
            role,
            resource,
            operation,
            AuthorizationRuleDecision.Allow);

        var resourceRepo = new FakeResourceRepository(new[] { resource });
        var userRoleAssignmentRepo = new FakeUserRoleAssignmentRepository(Array.Empty<string>());
        var ruleRepo = new FakeAuthorizationRuleRepository(new[] { rule }, userRoleAssignmentRepo);
        var engine = new AuthorizationEngine(ruleRepo, resourceRepo);

        // Act
        var result = await engine.EvaluateAsync(userId, resourceId, operationId);

        // Assert
        result.IsAuthorized.Should().BeFalse();
        result.UserId.Should().Be(userId);
    }

    [Fact]
    public async Task EvaluateAsync_Should_Reject_Inactive_Rules()
    {
        // Arrange
        var userId = "user-001";
        var resourceId = "resource-001";
        var operationId = "operation-001";

        var user = User.Create(userId, "tenant-001", UserType.EndUser);
        var resource = Resource.CreateRoot(resourceId, "app-001", "Dashboard", "dashboard");
        var operation = Operation.Create(operationId, "app-001", "read", "Read");
        var rule = AuthorizationRule.CreateForUser(
            "rule-001",
            user,
            resource,
            operation,
            AuthorizationRuleDecision.Allow);
        rule.Deactivate();

        var resourceRepo = new FakeResourceRepository(new[] { resource });
        var userRoleAssignmentRepo = new FakeUserRoleAssignmentRepository(Array.Empty<string>());
        var ruleRepo = new FakeAuthorizationRuleRepository(new[] { rule }, userRoleAssignmentRepo);
        var engine = new AuthorizationEngine(ruleRepo, resourceRepo);

        // Act
        var result = await engine.EvaluateAsync(userId, resourceId, operationId);

        // Assert
        result.IsAuthorized.Should().BeFalse();
        result.AppliedRules.Should().BeEmpty();
        result.UserId.Should().Be(userId);
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
        private readonly IUserRoleAssignmentRepository _userRoleAssignmentRepository;

        public FakeAuthorizationRuleRepository(IEnumerable<AuthorizationRule> rules, IUserRoleAssignmentRepository userRoleAssignmentRepository)
        {
            _rules = rules.ToList();
            _userRoleAssignmentRepository = userRoleAssignmentRepository;
        }

        public Task<IReadOnlyCollection<AuthorizationRule>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            var result = _rules.Where(r => r.UserId == userId).ToList();
            return Task.FromResult<IReadOnlyCollection<AuthorizationRule>>(result);
        }

        public async Task<IReadOnlyCollection<AuthorizationRule>> GetApplicableRulesAsync(
            string userId,
            string resourceId,
            string operationId,
            CancellationToken cancellationToken = default)
        {
            var userRoleAssignments = await _userRoleAssignmentRepository.GetByUserIdAsync(userId, cancellationToken);
            var roleIds = userRoleAssignments.Select(ra => ra.RoleId).ToArray();

            var result = _rules.Where(r =>
                r.IsActive &&
                r.ResourceId == resourceId &&
                r.OperationId == operationId &&
                (
                    (r.AppliesToUserAndRole && r.UserId == userId && roleIds.Contains(r.RoleId!)) ||
                    (r.AppliesToUserOnly && r.UserId == userId) ||
                    (r.AppliesToRoleOnly && roleIds.Contains(r.RoleId!))
                )).ToList();
            return result;
        }
    }

    private sealed class FakeUserRoleAssignmentRepository : IUserRoleAssignmentRepository
    {
        private readonly List<UserRoleAssignment> _assignments;

        public FakeUserRoleAssignmentRepository(IEnumerable<string> roleIds)
        {
            var tenantId = "tenant-001";
            var user = User.Create("user-001", tenantId, UserType.EndUser);
            _assignments = roleIds.Select(rid =>
            {
                var role = Role.Create(rid, tenantId, "Role " + rid);
                return UserRoleAssignment.Assign("assign-" + rid, user, role);
            }).ToList();
        }

        public Task<IReadOnlyCollection<UserRoleAssignment>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            var result = _assignments.Where(a => a.UserId == userId).ToList();
            return Task.FromResult<IReadOnlyCollection<UserRoleAssignment>>(result);
        }
    }
}
