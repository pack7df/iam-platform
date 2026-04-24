using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;
using Respawn;
using System.Data.Common;
using Npgsql;

namespace IamPlatform.IntegrationTests;

public abstract class BaseIntegrationTest : IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
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
                    // Here we will override the DB registration later in Phase 1
                });
            });

        Client = Factory.CreateClient();
        
        _dbConnection = new NpgsqlConnection(_dbContainer.GetConnectionString());
        await _dbConnection.OpenAsync();

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
}
