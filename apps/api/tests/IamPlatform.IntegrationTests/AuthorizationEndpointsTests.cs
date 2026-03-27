using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using IamPlatform.Application.Authorization.Rules;
using IamPlatform.Domain.Authorization;
using IamPlatform.Domain.Tenants;
using IamPlatform.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using DomainApp = IamPlatform.Domain.Tenants.Application;

namespace IamPlatform.IntegrationTests;

public sealed class AuthorizationEndpointsTests
{
    private async Task<(string ResourceId, string OperationId)> SeedDependencies(ApiWebApplicationFactory factory)
    {
        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IamPlatformDbContext>();

        var tenantId = Guid.NewGuid().ToString();
        var appId = Guid.NewGuid().ToString();
        var resId = Guid.NewGuid().ToString();
        var opId = Guid.NewGuid().ToString();

        var tenant = Tenant.Create(tenantId, "Test Tenant Auth");
        var app = DomainApp.Create(appId, tenantId, "Test App Auth");
        var resource = Resource.Create(resId, appId, "test-res", "Test Resource");
        var operation = Operation.Create(opId, appId, "test-op", "Test Operation");

        context.Tenants.Add(tenant);
        context.Applications.Add(app);
        context.Resources.Add(resource);
        context.Operations.Add(operation);
        await context.SaveChangesAsync();

        return (resId, opId);
    }

    [Fact]
    public async Task PostRule_Should_Return_Created()
    {
        await using var factory = new ApiWebApplicationFactory();
        using var client = factory.CreateClient();
        var (resId, opId) = await SeedDependencies(factory);

        var command = new CreateAuthorizationRuleCommand(
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(), // Random user ID for test
            null,
            resId,
            opId,
            "Allow");

        var response = await client.PostAsJsonAsync("/api/authorization/rules", command);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task GetRule_Should_Return_Ok_When_Exists()
    {
        await using var factory = new ApiWebApplicationFactory();
        using var client = factory.CreateClient();
        var (resId, opId) = await SeedDependencies(factory);

        var id = Guid.NewGuid().ToString();
        var command = new CreateAuthorizationRuleCommand(id, null, "admin", resId, opId, "Allow");
        await client.PostAsJsonAsync("/api/authorization/rules", command);
        
        var response = await client.GetAsync($"/api/authorization/rules/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<AuthorizationRuleResponse>();
        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
    }

    [Fact]
    public async Task ListRules_Should_Return_List()
    {
        await using var factory = new ApiWebApplicationFactory();
        using var client = factory.CreateClient();
        var (resId, opId) = await SeedDependencies(factory);

        var command1 = new CreateAuthorizationRuleCommand(Guid.NewGuid().ToString(), null, "role1", resId, opId, "Allow");
        var command2 = new CreateAuthorizationRuleCommand(Guid.NewGuid().ToString(), null, "role2", resId, opId, "Deny");

        await client.PostAsJsonAsync("/api/authorization/rules", command1);
        await client.PostAsJsonAsync("/api/authorization/rules", command2);
        
        var response = await client.GetAsync($"/api/authorization/rules?resourceId={resId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<AuthorizationRuleResponse>>();
        result.Should().NotBeNull();
        result!.Count.Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task PutRule_Should_Return_NoContent()
    {
        await using var factory = new ApiWebApplicationFactory();
        using var client = factory.CreateClient();
        var (resId, opId) = await SeedDependencies(factory);

        var id = Guid.NewGuid().ToString();
        var command = new CreateAuthorizationRuleCommand(id, "user1", null, resId, opId, "Allow");
        await client.PostAsJsonAsync("/api/authorization/rules", command);

        var updateCommand = new UpdateAuthorizationRuleCommand(id, "Deny", false);
        var response = await client.PutAsJsonAsync($"/api/authorization/rules/{id}", updateCommand);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        // Verify update
        var getResponse = await client.GetAsync($"/api/authorization/rules/{id}");
        var result = await getResponse.Content.ReadFromJsonAsync<AuthorizationRuleResponse>();
        result!.Decision.Should().Be("Deny");
        result!.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteRule_Should_Return_NoContent()
    {
        await using var factory = new ApiWebApplicationFactory();
        using var client = factory.CreateClient();
        var (resId, opId) = await SeedDependencies(factory);

        var id = Guid.NewGuid().ToString();
        var command = new CreateAuthorizationRuleCommand(id, "user1", null, resId, opId, "Allow");
        await client.PostAsJsonAsync("/api/authorization/rules", command);

        var response = await client.DeleteAsync($"/api/authorization/rules/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        // Verify deletion
        var getResponse = await client.GetAsync($"/api/authorization/rules/{id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
