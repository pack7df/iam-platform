using IamPlatform.Domain.Applications;
using IamPlatform.Domain.Common;
using IamPlatform.Domain.Tenants;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace IamPlatform.IntegrationTests.Applications;

public class ApplicationAndResourcePersistenceTests : BaseIntegrationTest
{
    [Fact]
    public async Task Should_PersistAndRetrieve_Application_And_Resources_UsingTenantScopedRepository()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var tenantRepo = scope.ServiceProvider.GetRequiredService<ITenantRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var tenant = new Tenant("App Tenant", "app-tenant");
        await tenantRepo.AddAsync(tenant);
        await unitOfWork.SaveChangesAsync();

        var appRepo = tenantRepo.GetApplicationRepository(tenant.Id);

        var app = new IamPlatform.Domain.Applications.Application(tenant.Id, "Core IAM", "core-iam")
        {
            Description = "The core identity application"
        };

        // Act - Save App
        await appRepo.AddAsync(app);
        await unitOfWork.SaveChangesAsync(); // Need Id for ResourceRepo Builder

        // Get Resource Repo from App Repo Builder
        var resRepo = appRepo.GetResourceRepository(app.Id);

        // Save Hierarchy of Resources
        var parentResource = new Resource(app.Id, "Folders", "folders");
        await resRepo.AddAsync(parentResource);
        await unitOfWork.SaveChangesAsync(); // Need Id for child

        var childResource = new Resource(app.Id, "Read Folders", "folders:read", parentResource.Id);
        await resRepo.AddAsync(childResource);
        await unitOfWork.SaveChangesAsync();

        // Assert
        var retrievedApp = await appRepo.GetBySlugAsync("core-iam");
        retrievedApp.Should().NotBeNull();
        retrievedApp!.Name.Should().Be("Core IAM");

        var allResources = (await resRepo.GetAllAsync()).ToList();
        allResources.Should().HaveCount(2);

        var retrievedChild = await resRepo.GetByKeyAsync("folders:read");
        retrievedChild.Should().NotBeNull();
        retrievedChild!.ParentId.Should().Be(parentResource.Id);

    }
}
