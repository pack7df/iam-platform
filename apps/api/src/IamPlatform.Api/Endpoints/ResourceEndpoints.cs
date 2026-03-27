using IamPlatform.Application.Authorization.Resources;
using MediatR;
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

        group.MapPost("/", async (CreateResourceCommand command, ISender mediator, CancellationToken ct) =>
        {
            await mediator.Send(command, ct);
            return Results.Created($"/api/resources/{command.Id}", command);
        });

        group.MapGet("/", async ([FromQuery] string applicationId, ISender mediator, CancellationToken ct) =>
        {
            var query = new ListResourcesQuery(applicationId);
            var resources = await mediator.Send(query, ct);
            return Results.Ok(resources);
        });

        group.MapGet("/{id}", async (string id, ISender mediator, CancellationToken ct) =>
        {
            var query = new GetResourceQuery(id);
            var resource = await mediator.Send(query, ct);
            return resource != null ? Results.Ok(resource) : Results.NotFound();
        });

        group.MapPut("/{id}", async (string id, UpdateResourceCommand command, ISender mediator, CancellationToken ct) =>
        {
            if (id != command.Id) return Results.BadRequest("ID mismatch.");
            await mediator.Send(command, ct);
            return Results.NoContent();
        });

        group.MapDelete("/{id}", async (string id, ISender mediator, CancellationToken ct) =>
        {
            var command = new DeleteResourceCommand(id);
            await mediator.Send(command, ct);
            return Results.NoContent();
        });

        return endpoints;
    }
}
