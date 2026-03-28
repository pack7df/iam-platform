using IamPlatform.Application.Authorization.Evaluation;
using IamPlatform.Application.Authorization.Rules;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace IamPlatform.Api.Endpoints;

public static class AuthorizationEndpoints
{
    public static IEndpointRouteBuilder MapAuthorizationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/authorization/evaluate", async (EvaluateAuthorizationRequest request, ISender mediator, CancellationToken ct) =>
        {
            var query = new EvaluateAuthorizationQuery(request.UserId, request.ResourceId, request.OperationId);
            var result = await mediator.Send(query, ct);
            return Results.Ok(result);
        }).WithTags("Authorization");

        var group = endpoints.MapGroup("/api/authorization/rules").WithTags("Authorization Rules");

        group.MapPost("/", async (CreateAuthorizationRuleCommand command, ISender mediator, CancellationToken ct) =>
        {
            await mediator.Send(command, ct);
            return Results.Created($"/api/authorization/rules/{command.Id}", command);
        });

        group.MapGet("/", async ([FromQuery] string? resourceId, [FromQuery] string? operationId, ISender mediator, CancellationToken ct) =>
        {
            var query = new ListAuthorizationRulesQuery(resourceId, operationId);
            var rules = await mediator.Send(query, ct);
            return Results.Ok(rules);
        });

        group.MapGet("/{id}", async (string id, ISender mediator, CancellationToken ct) =>
        {
            var query = new GetAuthorizationRuleQuery(id);
            var rule = await mediator.Send(query, ct);
            return rule != null ? Results.Ok(rule) : Results.NotFound();
        });

        group.MapPut("/{id}", async (string id, UpdateAuthorizationRuleCommand command, ISender mediator, CancellationToken ct) =>
        {
            if (id != command.Id) return Results.BadRequest("ID mismatch.");
            await mediator.Send(command, ct);
            return Results.NoContent();
        });

        group.MapDelete("/{id}", async (string id, ISender mediator, CancellationToken ct) =>
        {
            var command = new DeleteAuthorizationRuleCommand(id);
            await mediator.Send(command, ct);
            return Results.NoContent();
        });

        return endpoints;
    }
}

public record EvaluateAuthorizationRequest(string UserId, string ResourceId, string OperationId);
