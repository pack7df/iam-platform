using IamPlatform.Api;
using IamPlatform.Api.Endpoints;
using IamPlatform.Application;
using IamPlatform.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();

app.MapGet("/", () => Results.Ok(new { service = "IamPlatform.Api" }));
app.MapHealthChecks("/health");

// Endpoint Modules
app.MapBootstrapEndpoints();
app.MapTenantEndpoints();
app.MapInvitationEndpoints();
app.MapOperationEndpoints();
app.MapAuthorizationEndpoints();
app.MapResourceEndpoints();
app.MapUserRoleEndpoints();

app.Run();

public partial class Program;
