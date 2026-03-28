using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using IamPlatform.Domain.Tenants;
using IamPlatform.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IamPlatform.IntegrationTests;

public sealed class UserRoleEndpointsTests
{
    private async Task<(string UserId, string RoleId)> SeedUserAndRole(ApiWebApplicationFactory factory, bool sameTenant = true)
    {
        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IamPlatformDbContext>();

        var tenant1Id = Guid.NewGuid().ToString();
        var tenant2Id = Guid.NewGuid().ToString();
        
        var tenant1 = Tenant.Create(tenant1Id, "Tenant 1");
        context.Tenants.Add(tenant1);

        if (!sameTenant)
        {
            var tenant2 = Tenant.Create(tenant2Id, "Tenant 2");
            context.Tenants.Add(tenant2);
        }

        var userId = Guid.NewGuid().ToString();
        var roleId = Guid.NewGuid().ToString();

        var user = User.Create(userId, tenant1Id, UserType.EndUser);
        var role = Role.Create(roleId, sameTenant ? tenant1Id : tenant2Id, "Test Role");

        context.Users.Add(user);
        context.Roles.Add(role);
        await context.SaveChangesAsync();

        return (userId, roleId);
    }

    [Fact]
    public async Task PostUserRole_Should_Return_Created()
    {
        await using var factory = new ApiWebApplicationFactory();
        using var client = factory.CreateClient();
        var (userId, roleId) = await SeedUserAndRole(factory);

        var response = await client.PostAsJsonAsync($"/api/users/{userId}/roles", new { roleId });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task PostUserRole_Should_Return_BadRequest_When_TenantMismatch()
    {
        await using var factory = new ApiWebApplicationFactory();
        using var client = factory.CreateClient();
        var (userId, roleId) = await SeedUserAndRole(factory, sameTenant: false);

        var response = await client.PostAsJsonAsync($"/api/users/{userId}/roles", new { roleId });

        // Should fail due to domain validation (user and role in different tenants)
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetUserRoles_Should_Return_Ok()
    {
        await using var factory = new ApiWebApplicationFactory();
        using var client = factory.CreateClient();
        var (userId, roleId) = await SeedUserAndRole(factory);

        // Assign
        await client.PostAsJsonAsync($"/api/users/{userId}/roles", new { roleId });

        var response = await client.GetAsync($"/api/users/{userId}/roles");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var roles = await response.Content.ReadFromJsonAsync<List<RoleResponse>>();
        roles.Should().NotBeNull();
        roles.Should().Contain(r => r.Id == roleId);
    }

    [Fact]
    public async Task DeleteUserRole_Should_Return_NoContent()
    {
        await using var factory = new ApiWebApplicationFactory();
        using var client = factory.CreateClient();
        var (userId, roleId) = await SeedUserAndRole(factory);

        // Assign
        await client.PostAsJsonAsync($"/api/users/{userId}/roles", new { roleId });

        // Remove
        var response = await client.DeleteAsync($"/api/users/{userId}/roles/{roleId}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        // Verify removal
        var getResponse = await client.GetAsync($"/api/users/{userId}/roles");
        var roles = await getResponse.Content.ReadFromJsonAsync<List<RoleResponse>>();
        roles.Should().NotContain(r => r.Id == roleId);
    }

    private record RoleResponse(string Id, string Name);
}
