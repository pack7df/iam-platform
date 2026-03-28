using MediatR;

namespace IamPlatform.Application.Authorization.Roles;

public record AssignRoleToUserCommand(string UserId, string RoleId) : IRequest;

public record RemoveRoleFromUserCommand(string UserId, string RoleId) : IRequest;

public record GetUserRolesQuery(string UserId) : IRequest<List<UserRoleResponse>>;

public record UserRoleResponse(string Id, string Name);
