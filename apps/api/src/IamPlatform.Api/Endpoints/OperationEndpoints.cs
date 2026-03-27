using IamPlatform.Application.Authorization.Operations;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace IamPlatform.Api.Endpoints;

public static class OperationEndpoints
{
    public static IEndpointRouteBuilder MapOperationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/operations").WithTags("Operations");

        group.MapPost("/", async (CreateOperationCommand command, ISender mediator, CancellationToken ct) =>
        {
            await mediator.Send(command, ct);
            return Results.Created($"/api/operations/{command.Id}", command);
        });

        group.MapGet("/", async ([FromQuery] string applicationId, ISender mediator, CancellationToken ct) =>
        {
            var query = new ListOperationsQuery(applicationId);
            var operations = await mediator.Send(query, ct);
            return Results.Ok(operations);
        });

        group.MapGet("/{id}", async (string id, ISender mediator, CancellationToken ct) =>
        {
            var query = new GetOperationQuery(id);
            var operation = await mediator.Send(query, ct);
            return operation != null ? Results.Ok(operation) : Results.NotFound();
        });

        group.MapPut("/{id}", async (string id, UpdateOperationCommand command, ISender mediator, CancellationToken ct) =>
        {
            if (id != command.Id) return Results.BadRequest("ID mismatch.");
            await mediator.Send(command, ct);
            return Results.NoContent();
        });

        group.MapDelete("/{id}", async (string id, ISender mediator, CancellationToken ct) =>
        {
            var command = new DeleteOperationCommand(id);
            await mediator.Send(command, ct);
            return Results.NoContent();
        });

        return endpoints;
    }
}
