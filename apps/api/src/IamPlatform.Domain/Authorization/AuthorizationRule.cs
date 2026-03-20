using IamPlatform.Domain.Primitives;
using IamPlatform.Domain.Tenants;

namespace IamPlatform.Domain.Authorization;

public sealed class AuthorizationRule
{
    private AuthorizationRule(
        string id,
        string resourceId,
        string operationId,
        string? tenantUserId,
        string? roleId,
        AuthorizationRuleDecision decision,
        bool isActive)
    {
        Id = Guard.Required(id, nameof(id), "Authorization rule id is required.");
        ResourceId = Guard.Required(resourceId, nameof(resourceId), "Resource id is required.");
        OperationId = Guard.Required(operationId, nameof(operationId), "Operation id is required.");
        TenantUserId = NormalizeOptional(tenantUserId);
        RoleId = NormalizeOptional(roleId);
        Decision = decision;
        IsActive = isActive;
    }

    public string Id { get; }

    public string ResourceId { get; }

    public string OperationId { get; }

    public string? TenantUserId { get; }

    public string? RoleId { get; }

    public AuthorizationRuleDecision Decision { get; private set; }

    public bool IsActive { get; private set; }

    public bool AppliesToTenantUserOnly => TenantUserId is not null && RoleId is null;

    public bool AppliesToRoleOnly => TenantUserId is null && RoleId is not null;

    public bool AppliesToTenantUserAndRole => TenantUserId is not null && RoleId is not null;

    public static AuthorizationRule CreateForTenantUser(
        string id,
        TenantUser tenantUser,
        Resource resource,
        Operation operation,
        AuthorizationRuleDecision decision)
    {
        ArgumentNullException.ThrowIfNull(tenantUser);

        EnsureSameApplication(resource, operation);

        return Create(id, resource.Id, operation.Id, tenantUser.Id, null, decision);
    }

    public static AuthorizationRule CreateForRole(
        string id,
        Role role,
        Resource resource,
        Operation operation,
        AuthorizationRuleDecision decision)
    {
        ArgumentNullException.ThrowIfNull(role);

        EnsureSameApplication(resource, operation);

        return Create(id, resource.Id, operation.Id, null, role.Id, decision);
    }

    public static AuthorizationRule CreateForTenantUserAndRole(
        string id,
        TenantUser tenantUser,
        Role role,
        Resource resource,
        Operation operation,
        AuthorizationRuleDecision decision)
    {
        ArgumentNullException.ThrowIfNull(tenantUser);
        ArgumentNullException.ThrowIfNull(role);

        if (tenantUser.TenantId != role.TenantId)
        {
            throw new InvalidOperationException("Tenant user and role must belong to the same tenant.");
        }

        EnsureSameApplication(resource, operation);

        return Create(id, resource.Id, operation.Id, tenantUser.Id, role.Id, decision);
    }

    public void ChangeDecision(AuthorizationRuleDecision decision)
    {
        Decision = decision;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    private static AuthorizationRule Create(
        string id,
        string resourceId,
        string operationId,
        string? tenantUserId,
        string? roleId,
        AuthorizationRuleDecision decision)
    {
        var normalizedTenantUserId = NormalizeOptional(tenantUserId);
        var normalizedRoleId = NormalizeOptional(roleId);

        if (normalizedTenantUserId is null && normalizedRoleId is null)
        {
            throw new InvalidOperationException("Authorization rule must target at least one tenant user or role.");
        }

        return new AuthorizationRule(
            id,
            resourceId,
            operationId,
            normalizedTenantUserId,
            normalizedRoleId,
            decision,
            true);
    }

    private static void EnsureSameApplication(Resource resource, Operation operation)
    {
        ArgumentNullException.ThrowIfNull(resource);
        ArgumentNullException.ThrowIfNull(operation);

        if (resource.ApplicationId != operation.ApplicationId)
        {
            throw new InvalidOperationException("Resource and operation must belong to the same application.");
        }
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
