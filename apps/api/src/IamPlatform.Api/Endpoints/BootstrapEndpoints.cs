using IamPlatform.Application.Identity;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace IamPlatform.Api.Endpoints;

public static class BootstrapEndpoints
{
    public static IEndpointRouteBuilder MapBootstrapEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/bootstrap/system-user", async (BootstrapSystemUserCommand request, ISender mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(request, cancellationToken);

            if (!result.IsCreated && result.User == null)
            {
                return Results.Conflict(new { message = result.Message });
            }

            return Results.Created($"/bootstrap/system-user/{result.User!.Id}", new
            {
                id = result.User.Id,
                type = result.User.Type.ToString()
            });
        })
        .WithTags("Bootstrap");

        return endpoints;
    }
}
