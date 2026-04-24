using IamPlatform.Domain.Tenants;
using IamPlatform.Domain.Common;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace IamPlatform.IntegrationTests.Tenants;

public class TenantPersistenceTests : BaseIntegrationTest
{
    [Fact]
    public async Task Should_PersistAndRetrieve_Tenant_UsingRepository()
    {
        // Arrange
        var tenant = new Tenant("Repo Tenant", "repo-tenant");
        using var scope = Factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ITenantRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        // Act
        await repository.AddAsync(tenant);
        await unitOfWork.SaveChangesAsync();

        // Assert
        var allTenants = await repository.GetAllAsync();
        var retrieved = allTenants.FirstOrDefault(t => t.Slug == "repo-tenant");
        
        retrieved.Should().NotBeNull();
        retrieved!.Name.Should().Be("Repo Tenant");
    }
}


