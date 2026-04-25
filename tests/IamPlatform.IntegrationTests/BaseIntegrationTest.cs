using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using IamPlatform.Infrastructure.Persistence;
using IamPlatform.Domain.Common;
using IamPlatform.Infrastructure.Persistence.Interceptors;
using Testcontainers.PostgreSql;
using Respawn;
using System.Data.Common;
using Npgsql;


namespace IamPlatform.IntegrationTests;

public abstract class BaseIntegrationTest : IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder("postgres:16-alpine")
        .WithDatabase("iam_db")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();


    protected WebApplicationFactory<Program> Factory { get; private set; } = default!;
    protected HttpClient Client { get; private set; } = default!;
    private Respawner _respawner = default!;
    private DbConnection _dbConnection = default!;

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        Factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<IamPlatformDbContext>));

                    if (descriptor != null) services.Remove(descriptor);

                    services.AddDbContext<IamPlatformDbContext>((sp, options) =>
                    {
                        var auditInterceptor = sp.GetRequiredService<AuditInterceptor>();
                        options.UseNpgsql(_dbContainer.GetConnectionString())
                               .AddInterceptors(auditInterceptor);
                    });

                    services.AddScoped<IUserContext, TestUserContext>();
                });
            });


        Client = Factory.CreateClient();
        
        _dbConnection = new NpgsqlConnection(_dbContainer.GetConnectionString());
        await _dbConnection.OpenAsync();

        // Ensure database schema is created
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IamPlatformDbContext>();
        await context.Database.EnsureCreatedAsync();

        _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions

        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = new[] { "public" }
        });
    }

    public async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_dbConnection);
    }

    public async Task DisposeAsync()
    {
        await _dbConnection.CloseAsync();
        await _dbContainer.StopAsync();
    }

    private class TestUserContext : IUserContext
    {
        public Guid? UserId => null;
    }
}

