using IamPlatform.Application.Authorization.Roles;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace IamPlatform.Api.Endpoints;

public static class UserRoleEndpoints
{
    public static IEndpointRouteBuilder MapUserRoleEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/users/{userId}/roles").WithTags("User Roles");

        group.MapPost("/", async (string userId, AssignRoleRequest request, ISender mediator, CancellationToken ct) =>
        {
            try
            {
                var command = new AssignRoleToUserCommand(userId, request.RoleId);
                await mediator.Send(command, ct);
                return Results.Created($"/api/users/{userId}/roles", new { userId, request.RoleId });
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(ex.Message);
            }
        });

        group.MapGet("/", async (string userId, ISender mediator, CancellationToken ct) =>
        {
            var query = new GetUserRolesQuery(userId);
            var roles = await mediator.Send(query, ct);
            return Results.Ok(roles);
        });

        group.MapDelete("/{roleId}", async (string userId, string roleId, ISender mediator, CancellationToken ct) =>
        {
            var command = new RemoveRoleFromUserCommand(userId, roleId);
            await mediator.Send(command, ct);
            return Results.NoContent();
        });

        return endpoints;
    }
}

public record AssignRoleRequest(string RoleId);
