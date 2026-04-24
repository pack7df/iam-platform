using IamPlatform.Domain.Tenants;
using IamPlatform.Domain.Users;
using IamPlatform.Domain.Common;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace IamPlatform.IntegrationTests.Users;

public class UserPersistenceTests : BaseIntegrationTest
{
    [Fact]
    public async Task Should_PersistAndRetrieve_User_UsingTenantScopedRepository()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var tenantRepo = scope.ServiceProvider.GetRequiredService<ITenantRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var tenant = new Tenant("Scoped Tenant", "scoped-tenant");
        await tenantRepo.AddAsync(tenant);
        await unitOfWork.SaveChangesAsync();

        // Get scoped repository
        var userRepo = tenantRepo.GetUserRepository(tenant.Id);

        var user = new User(tenant.Id, "scoped_user", "scoped@example.com", "hash456");

        // Act
        await userRepo.AddAsync(user);
        await unitOfWork.SaveChangesAsync();

        // Assert
        var retrieved = await userRepo.GetByEmailAsync("scoped@example.com");
        
        retrieved.Should().NotBeNull();
        retrieved!.Username.Should().Be("scoped_user");
        retrieved.TenantId.Should().Be(tenant.Id);
    }

}
