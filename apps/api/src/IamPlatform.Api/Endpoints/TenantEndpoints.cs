using IamPlatform.Application.Tenants;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace IamPlatform.Api.Endpoints;

public static class TenantEndpoints
{
    public static IEndpointRouteBuilder MapTenantEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/tenants/registration", async (RegisterTenantAdminRequest request, ITenantAdminRegistration registration, CancellationToken cancellationToken) =>
        {
            var result = await registration.RegisterAsync(request.TenantId, request.TenantName, request.TenantAdminId, cancellationToken);

            return Results.Created($"/tenants/{result.Tenant.Id}", new
            {
                tenant = new { id = result.Tenant.Id, name = result.Tenant.Name, isActive = result.Tenant.IsActive },
                tenantAdmin = new { id = result.TenantAdmin.Id, tenantId = result.TenantAdmin.TenantId, type = result.TenantAdmin.Type.ToString(), isActive = result.TenantAdmin.IsActive }
            });
        })
        .WithTags("Tenants");

        return endpoints;
    }
}
