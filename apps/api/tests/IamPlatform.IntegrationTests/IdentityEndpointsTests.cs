using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace IamPlatform.IntegrationTests;

public sealed class IdentityEndpointsTests
{
    [Fact]
    public async Task PostBootstrapSystemUser_Should_Create_First_SystemUser()
    {
        await using var factory = new ApiWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/bootstrap/system-user", new { systemUserId = "system-user-001" });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task PostBootstrapSystemUser_Should_ReturnConflict_When_Bootstrap_Already_Completed()
    {
        await using var factory = new ApiWebApplicationFactory();
        using var client = factory.CreateClient();

        await client.PostAsJsonAsync("/bootstrap/system-user", new { systemUserId = "system-user-001" });
        var response = await client.PostAsJsonAsync("/bootstrap/system-user", new { systemUserId = "system-user-002" });

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task PostTenantsRegistration_Should_Create_Tenant_And_TenantAdmin()
    {
        await using var factory = new ApiWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/tenants/registration", new
        {
            tenantId = "tenant-001",
            tenantName = "Acme Corp",
            tenantAdminId = "tenant-admin-001"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task PostSystemUserInvitations_Should_Create_Pending_Invitation()
    {
        await using var factory = new ApiWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/system-user-invitations", new
        {
            invitationId = "invite-001",
            invitedSystemUserId = "system-user-002"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task PostTenantAdminInvitations_Should_Create_Pending_Invitation_For_Tenant()
    {
        await using var factory = new ApiWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/tenant-admin-invitations", new
        {
            invitationId = "invite-001",
            tenantId = "tenant-001",
            invitedTenantAdminId = "tenant-admin-002"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
