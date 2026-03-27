namespace IamPlatform.Application.Authorization.Resources;

public sealed record ResourceResponse(
    string Id,
    string Name,
    string? ParentId,
    string ApplicationId,
    bool IsActive);
