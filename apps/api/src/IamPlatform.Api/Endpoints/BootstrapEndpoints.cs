using IamPlatform.Application.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace IamPlatform.Api.Endpoints;

public static class BootstrapEndpoints
{
    public static IEndpointRouteBuilder MapBootstrapEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/bootstrap/system-user", async (BootstrapSystemUserRequest request, ISystemUserBootstrapper bootstrapper, CancellationToken cancellationToken) =>
        {
            var result = await bootstrapper.BootstrapAsync(cancellationToken);

            return result.IsCreated
                ? Results.Created($"/system-users/{result.SystemUser!.Id}", new
                {
                    id = result.SystemUser.Id,
                    isActive = result.SystemUser.IsActive,
                    bootstrapCompleted = true
                })
                : Results.Conflict(new { message = "Initial system user bootstrap has already been completed." });
        })
        .WithTags("Bootstrap");

        return endpoints;
    }
}
