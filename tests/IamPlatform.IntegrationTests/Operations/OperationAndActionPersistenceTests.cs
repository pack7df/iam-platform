using IamPlatform.Domain.Applications;
using IamPlatform.Domain.Operations;
using IamPlatform.Domain.Tenants;
using IamPlatform.Domain.Common;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Action = IamPlatform.Domain.Applications.Action;


namespace IamPlatform.IntegrationTests.Operations;

public class OperationAndActionPersistenceTests : BaseIntegrationTest
{
    [Fact]
    public async Task Should_PersistAndRetrieve_Operations_And_Actions()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var tenantRepo = scope.ServiceProvider.GetRequiredService<ITenantRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var tenant = new Tenant("Op Tenant", "op-tenant");
        await tenantRepo.AddAsync(tenant);
        await unitOfWork.SaveChangesAsync();

        var opRepo = tenantRepo.GetOperationRepository(tenant.Id);
        var appRepo = tenantRepo.GetApplicationRepository(tenant.Id);

        // Act - Create Operation (Tenant Level)
        var readOperation = new Operation(tenant.Id, "Read", "read");
        var writeOperation = new Operation(tenant.Id, "Write", "write");
        await opRepo.AddAsync(readOperation);
        await opRepo.AddAsync(writeOperation);

        // Create Application
        var app = new IamPlatform.Domain.Applications.Application(tenant.Id, "Doc App", "doc-app");
        await appRepo.AddAsync(app);
        await unitOfWork.SaveChangesAsync(); // Need Id for Resource

        var resRepo = appRepo.GetResourceRepository(app.Id);

        // Create Resource
        var folderResource = new Resource(app.Id, "Folder", "folder");
        await resRepo.AddAsync(folderResource);
        await unitOfWork.SaveChangesAsync(); // Need Id for Action

        var actionRepo = resRepo.GetActionRepository(folderResource.Id);

        // Bridge: Action (Allow Read on Folder)
        var readAction = new Action(folderResource.Id, readOperation.Id);
        var writeAction = new Action(folderResource.Id, writeOperation.Id);
        
        await actionRepo.AddAsync(readAction);
        await actionRepo.AddAsync(writeAction);
        await unitOfWork.SaveChangesAsync();

        // Assert
        var retrievedOp = await opRepo.GetByKeyAsync("read");
        retrievedOp.Should().NotBeNull();
        retrievedOp!.Name.Should().Be("Read");

        var actions = (await actionRepo.GetAllAsync()).ToList();
        actions.Should().HaveCount(2);
        
        var retrievedAction = await actionRepo.GetByOperationAsync(readOperation.Id);
        retrievedAction.Should().NotBeNull();
        retrievedAction!.OperationId.Should().Be(readOperation.Id);
    }
}
