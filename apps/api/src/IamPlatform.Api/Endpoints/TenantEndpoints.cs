using IamPlatform.Application.Tenants;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace IamPlatform.Api.Endpoints;

public static class TenantEndpoints
{
    public static IEndpointRouteBuilder MapTenantEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/tenants", async (RegisterTenantAdminCommand request, ISender mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(request, cancellationToken);

            return Results.Created($"/tenants/{result.Tenant.Id}", new
            {
                tenantId = result.Tenant.Id,
                tenantName = result.Tenant.Name,
                adminId = result.Admin.Id
            });
        })
        .WithTags("Tenants");

        return endpoints;
    }
}
