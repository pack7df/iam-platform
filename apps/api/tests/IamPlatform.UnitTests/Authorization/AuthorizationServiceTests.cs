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
        var resourceId = "resource-001";
        var operationId = "op-001";
        var applicationId = "app-001";

        var user = User.Create(userId, "tenant-001", UserType.EndUser);
        var resource = Resource.CreateRoot(resourceId, applicationId, "Test Resource", "test");
        var operation = Operation.Create("op-001", applicationId, "read", "Read");
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
        var service = new AuthorizationService(engine);

        // Act
        var result = await service.EvaluateAsync(userId, resourceId, operationId);

        // Assert
        result.IsAuthorized.Should().BeTrue();
        result.Decision.Should().Be(AuthorizationRuleDecision.Allow);
        result.UserId.Should().Be(userId);
        result.AppliedRules.Should().ContainSingle();
    }

    [Fact]
    public async Task EvaluateAsync_Should_Deny_When_No_Rules_Match()
    {
        // Arrange
        var userId = "user-001";
        var resourceId = "resource-001";
        var operationId = "op-001";
        var applicationId = "app-001";

        var resource = Resource.CreateRoot(resourceId, applicationId, "Test Resource", "test");
        var resourceRepo = new FakeResourceRepository(new[] { resource });
        var userRoleAssignmentRepo = new FakeUserRoleAssignmentRepository(Array.Empty<string>());
        var ruleRepo = new FakeAuthorizationRuleRepository(Array.Empty<AuthorizationRule>(), userRoleAssignmentRepo);
        var engine = new AuthorizationEngine(ruleRepo, resourceRepo);
        var service = new AuthorizationService(engine);

        // Act
        var result = await service.EvaluateAsync(userId, resourceId, operationId);

        // Assert
        result.IsAuthorized.Should().BeFalse();
        result.Decision.Should().Be(AuthorizationRuleDecision.Deny);
        result.UserId.Should().Be(userId);
        result.AppliedRules.Should().BeEmpty();
    }

    [Fact]
    public async Task EvaluateAsync_Should_Combine_User_And_Role_Rules_With_Deny_Precedence()
    {
        // Arrange
        var userId = "user-001";
        var resourceId = "resource-001";
        var operationId = "op-001";
        var roleId = "role-001";
        var applicationId = "app-001";

        var user = User.Create(userId, "tenant-001", UserType.EndUser);
        var role = Role.Create(roleId, "tenant-001", "Operators");
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
        var userRoleAssignmentRepo = new FakeUserRoleAssignmentRepository(new[] { roleId });
        var ruleRepo = new FakeAuthorizationRuleRepository(new[] { userAllowRule, roleDenyRule }, userRoleAssignmentRepo);
        var engine = new AuthorizationEngine(ruleRepo, resourceRepo);
        var service = new AuthorizationService(engine);

        // Act
        var result = await service.EvaluateAsync(userId, resourceId, operationId);

        // Assert
        result.IsAuthorized.Should().BeFalse(); // Deny takes precedence
        result.Decision.Should().Be(AuthorizationRuleDecision.Deny);
        result.UserId.Should().Be(userId);
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
        var userRoleAssignmentRepo = new FakeUserRoleAssignmentRepository(Array.Empty<string>());
        var ruleRepo = new FakeAuthorizationRuleRepository(new[] { inheritRule, parentAllowRule }, userRoleAssignmentRepo);
        var engine = new AuthorizationEngine(ruleRepo, resourceRepo);
        var service = new AuthorizationService(engine);

        // Act
        var result = await service.EvaluateAsync(userId, "resource-child", "op-read");

        // Assert
        result.IsAuthorized.Should().BeTrue();
        result.Decision.Should().Be(AuthorizationRuleDecision.Allow);
        result.ResolvedResourceId.Should().Be("resource-root");
        result.UserId.Should().Be(userId);
        result.AppliedRules.Should().Contain(parentAllowRule);
    }

    [Fact]
    public async Task EvaluateAsync_Should_Deny_When_Resource_Not_Found()
    {
        // Arrange
        var userId = "user-001";
        var resourceId = "non-existent";
        var operationId = "op-001";

        var resourceRepo = new FakeResourceRepository(Array.Empty<Resource>());
        var userRoleAssignmentRepo = new FakeUserRoleAssignmentRepository(Array.Empty<string>());
        var ruleRepo = new FakeAuthorizationRuleRepository(Array.Empty<AuthorizationRule>(), userRoleAssignmentRepo);
        var engine = new AuthorizationEngine(ruleRepo, resourceRepo);
        var service = new AuthorizationService(engine);

        // Act
        var result = await service.EvaluateAsync(userId, resourceId, operationId);

        // Assert
        result.IsAuthorized.Should().BeFalse();
        result.Decision.Should().Be(AuthorizationRuleDecision.Deny);
        result.UserId.Should().Be(userId);
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

        public Task AddAsync(Resource resource, CancellationToken cancellationToken = default)
        {
            _resources.Add(resource);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Resource resource, CancellationToken cancellationToken = default)
        {
            var existing = _resources.FirstOrDefault(r => r.Id == resource.Id);
            if (existing != null)
            {
                _resources.Remove(existing);
                _resources.Add(resource);
            }
            return Task.CompletedTask;
        }

        public Task RemoveAsync(Resource resource, CancellationToken cancellationToken = default)
        {
            var existing = _resources.FirstOrDefault(r => r.Id == resource.Id);
            if (existing != null)
            {
                _resources.Remove(existing);
            }
            return Task.CompletedTask;
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

        public Task<AuthorizationRule?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var result = _rules.FirstOrDefault(r => r.Id == id);
            return Task.FromResult<AuthorizationRule?>(result);
        }

        public Task<IReadOnlyCollection<AuthorizationRule>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyCollection<AuthorizationRule>>(_rules);
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

        public Task AddAsync(AuthorizationRule rule, CancellationToken cancellationToken = default)
        {
            _rules.Add(rule);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(AuthorizationRule rule, CancellationToken cancellationToken = default)
        {
            var existing = _rules.FirstOrDefault(r => r.Id == rule.Id);
            if (existing != null)
            {
                _rules.Remove(existing);
                _rules.Add(rule);
            }
            return Task.CompletedTask;
        }

        public Task RemoveAsync(AuthorizationRule rule, CancellationToken cancellationToken = default)
        {
            _rules.Remove(rule);
            return Task.CompletedTask;
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

        public Task AddAsync(UserRoleAssignment assignment, CancellationToken cancellationToken = default)
        {
            _assignments.Add(assignment);
            return Task.CompletedTask;
        }

        public Task RemoveAsync(UserRoleAssignment assignment, CancellationToken cancellationToken = default)
        {
            _assignments.Remove(assignment);
            return Task.CompletedTask;
        }
    }
}
