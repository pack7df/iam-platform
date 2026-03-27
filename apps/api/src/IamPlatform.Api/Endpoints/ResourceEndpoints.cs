using IamPlatform.Application.Authorization.Resources;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace IamPlatform.Api.Endpoints;

public static class ResourceEndpoints
{
    public static IEndpointRouteBuilder MapResourceEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/resources").WithTags("Resources");

        group.MapPost("/", async (CreateResourceCommand command, ICreateResourceHandler handler, CancellationToken ct) =>
        {
            await handler.HandleAsync(command, ct);
            return Results.Created($"/api/resources/{command.Id}", command);
        });

        group.MapGet("/", async ([FromQuery] string applicationId, IListResourcesHandler handler, CancellationToken ct) =>
        {
            var resources = await handler.HandleAsync(applicationId, ct);
            return Results.Ok(resources);
        });

        group.MapGet("/{id}", async (string id, IGetResourceHandler handler, CancellationToken ct) =>
        {
            var resource = await handler.HandleAsync(id, ct);
            return resource != null ? Results.Ok(resource) : Results.NotFound();
        });

        group.MapPut("/{id}", async (string id, UpdateResourceCommand command, IUpdateResourceHandler handler, CancellationToken ct) =>
        {
            if (id != command.Id) return Results.BadRequest("ID mismatch.");
            await handler.HandleAsync(command, ct);
            return Results.NoContent();
        });

        group.MapDelete("/{id}", async (string id, IDeleteResourceHandler handler, CancellationToken ct) =>
        {
            await handler.HandleAsync(id, ct);
            return Results.NoContent();
        });

        return endpoints;
    }
}
