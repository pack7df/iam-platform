using IamPlatform.Domain.Authorization;

namespace IamPlatform.Application.Authorization.Rules;

public sealed record AuthorizationRuleResponse(
    string Id,
    string? UserId,
    string? RoleId,
    string ResourceId,
    string OperationId,
    string Decision,
    bool IsActive);
