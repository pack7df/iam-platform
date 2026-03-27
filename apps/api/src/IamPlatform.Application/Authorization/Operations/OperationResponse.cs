namespace IamPlatform.Application.Authorization.Operations;

public sealed record OperationResponse(
    string Id,
    string Name,
    string Key,
    string ApplicationId,
    bool IsActive);
