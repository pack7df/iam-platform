using IamPlatform.Application.Identity;
using IamPlatform.Application.Tenants;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace IamPlatform.Api.Endpoints;

public static class InvitationEndpoints
{
    public static IEndpointRouteBuilder MapInvitationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/system-user-invitations", async (InviteSystemUserRequest request, ISystemUserInvitation invitationService, CancellationToken cancellationToken) =>
        {
            var result = await invitationService.InviteAsync(request.InvitationId, request.InvitedSystemUserId, cancellationToken);

            return Results.Created($"/system-user-invitations/{result.Invitation.Id}", new
            {
                id = result.Invitation.Id,
                invitedIdentityId = result.Invitation.InvitedIdentityId,
                targetType = result.Invitation.TargetType.ToString(),
                tenantId = result.Invitation.TenantId,
                status = result.Invitation.Status.ToString()
            });
        })
        .WithTags("Invitations");

        endpoints.MapPost("/tenant-admin-invitations", async (InviteTenantAdminRequest request, ITenantAdminInvitation invitationService, CancellationToken cancellationToken) =>
        {
            var result = await invitationService.InviteAsync(request.InvitationId, request.TenantId, request.InvitedTenantAdminId, cancellationToken);

            return Results.Created($"/tenant-admin-invitations/{result.Invitation.Id}", new
            {
                id = result.Invitation.Id,
                invitedIdentityId = result.Invitation.InvitedIdentityId,
                targetType = result.Invitation.TargetType.ToString(),
                tenantId = result.Invitation.TenantId,
                status = result.Invitation.Status.ToString()
            });
        })
        .WithTags("Invitations");

        return endpoints;
    }
}
