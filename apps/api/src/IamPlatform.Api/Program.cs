using IamPlatform.Application;
using IamPlatform.Application.Identity;
using IamPlatform.Application.Tenants;
using IamPlatform.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();
builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.MapGet("/", () => Results.Ok(new { service = "IamPlatform.Api" }));
app.MapHealthChecks("/health");
app.MapPost("/bootstrap/system-user", async (BootstrapSystemUserRequest request, ISystemUserBootstrapper bootstrapper, CancellationToken cancellationToken) =>
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
app.MapPost("/tenants/registration", async (RegisterTenantAdminRequest request, ITenantAdminRegistration registration, CancellationToken cancellationToken) =>
{
    var result = await registration.RegisterAsync(request.TenantId, request.TenantName, request.TenantAdminId, cancellationToken);

    return Results.Created($"/tenants/{result.Tenant.Id}", new
    {
        tenant = new { id = result.Tenant.Id, name = result.Tenant.Name, isActive = result.Tenant.IsActive },
        tenantAdmin = new { id = result.TenantAdmin.Id, tenantId = result.TenantAdmin.TenantId, type = result.TenantAdmin.Type.ToString(), isActive = result.TenantAdmin.IsActive }
    });
});
app.MapPost("/system-user-invitations", async (InviteSystemUserRequest request, ISystemUserInvitation invitationService, CancellationToken cancellationToken) =>
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
});
app.MapPost("/tenant-admin-invitations", async (InviteTenantAdminRequest request, ITenantAdminInvitation invitationService, CancellationToken cancellationToken) =>
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
});

app.Run();

public partial class Program;
