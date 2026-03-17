using IamPlatform.Application;
using IamPlatform.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();
builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.MapGet("/", () => Results.Ok(new { service = "IamPlatform.Api" }));
app.MapHealthChecks("/health");

app.Run();

public partial class Program;
