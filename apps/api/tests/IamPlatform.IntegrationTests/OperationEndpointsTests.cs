using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using IamPlatform.Application.Authorization.Operations;
using DomainApp = IamPlatform.Domain.Tenants.Application;
using IamPlatform.Domain.Tenants;
using IamPlatform.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IamPlatform.IntegrationTests;

public sealed class OperationEndpointsTests
{
    private async Task<(string TenantId, string ApplicationId)> SeedTenantAndApplication(ApiWebApplicationFactory factory)
    {
        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IamPlatformDbContext>();

        var tenantId = Guid.NewGuid().ToString();
        var appId = Guid.NewGuid().ToString();

        var tenant = Tenant.Create(tenantId, "Test Tenant " + tenantId);
        var app = DomainApp.Create(appId, tenantId, "Test App " + appId);

        context.Tenants.Add(tenant);
        context.Applications.Add(app);
        await context.SaveChangesAsync();

        return (tenantId, appId);
    }

    [Fact]
    public async Task PostOperation_Should_Return_Created()
    {
        await using var factory = new ApiWebApplicationFactory();
        using var client = factory.CreateClient();
        var (_, appId) = await SeedTenantAndApplication(factory);

        var command = new CreateOperationCommand(
            Guid.NewGuid().ToString(),
            "Tesing Operation",
            "test-op",
            appId);

        var response = await client.PostAsJsonAsync("/api/operations", command);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task GetOperation_Should_Return_Ok_When_Exists()
    {
        await using var factory = new ApiWebApplicationFactory();
        using var client = factory.CreateClient();
        var (_, appId) = await SeedTenantAndApplication(factory);

        var id = Guid.NewGuid().ToString();
        var command = new CreateOperationCommand(
            id,
            "Get Operation",
            "get-op",
            appId);

        await client.PostAsJsonAsync("/api/operations", command);
        
        var response = await client.GetAsync($"/api/operations/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<OperationResponse>();
        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
    }

    [Fact]
    public async Task GetOperations_Should_Return_List_For_Application()
    {
        await using var factory = new ApiWebApplicationFactory();
        using var client = factory.CreateClient();
        var (_, appId) = await SeedTenantAndApplication(factory);

        var command1 = new CreateOperationCommand(Guid.NewGuid().ToString(), "Op 1", "op-1", appId);
        var command2 = new CreateOperationCommand(Guid.NewGuid().ToString(), "Op 2", "op-2", appId);

        await client.PostAsJsonAsync("/api/operations", command1);
        await client.PostAsJsonAsync("/api/operations", command2);
        
        var response = await client.GetAsync($"/api/operations?applicationId={appId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<OperationResponse>>();
        result.Should().NotBeNull();
        result!.Count.Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task PutOperation_Should_Return_NoContent()
    {
        await using var factory = new ApiWebApplicationFactory();
        using var client = factory.CreateClient();
        var (_, appId) = await SeedTenantAndApplication(factory);

        var id = Guid.NewGuid().ToString();
        var command = new CreateOperationCommand(id, "Old Name", "old-key", appId);
        await client.PostAsJsonAsync("/api/operations", command);

        var updateCommand = new UpdateOperationCommand(id, "New Name", "new-key", false);
        var response = await client.PutAsJsonAsync($"/api/operations/{id}", updateCommand);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        // Verify update
        var getResponse = await client.GetAsync($"/api/operations/{id}");
        var result = await getResponse.Content.ReadFromJsonAsync<OperationResponse>();
        result!.Name.Should().Be("New Name");
        result!.Key.Should().Be("new-key");
        result!.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteOperation_Should_Return_NoContent()
    {
        await using var factory = new ApiWebApplicationFactory();
        using var client = factory.CreateClient();
        var (_, appId) = await SeedTenantAndApplication(factory);

        var id = Guid.NewGuid().ToString();
        var command = new CreateOperationCommand(id, "Delete me", "delete-op", appId);
        await client.PostAsJsonAsync("/api/operations", command);

        var response = await client.DeleteAsync($"/api/operations/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        // Verify deletion
        var getResponse = await client.GetAsync($"/api/operations/{id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
