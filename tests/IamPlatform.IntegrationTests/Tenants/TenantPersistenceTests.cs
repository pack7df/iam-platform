using IamPlatform.Domain.Tenants;
using IamPlatform.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IamPlatform.IntegrationTests.Tenants;

public class TenantPersistenceTests : BaseIntegrationTest
{
    [Fact]
    public async Task Should_PersistAndRetrieve_Tenant()
    {
        // Arrange
        var tenant = new Tenant("Test Tenant", "test-tenant");
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IamPlatformDbContext>();

        // Act
        context.Tenants.Add(tenant);
        await context.SaveChangesAsync();

        // Assert
        var retrieved = await context.Tenants.FirstOrDefaultAsync(t => t.Slug == "test-tenant");
        retrieved.Should().NotBeNull();
        retrieved!.Name.Should().Be("Test Tenant");
    }
}

