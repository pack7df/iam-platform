using IamPlatform.Domain.Common;
using IamPlatform.Domain.Tenants;
using IamPlatform.Domain.Users;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using IamPlatform.Infrastructure.Persistence;

namespace IamPlatform.IntegrationTests.Infrastructure;

public class AuditPersistenceTests : BaseIntegrationTest
{
    private class TestUserContext : IUserContext
    {
        public Guid? UserId { get; set; }
    }

    [Fact]
    public async Task Should_CreateAuditLog_When_EntityIsCreated()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IamPlatformDbContext>();
        
        var tenant = new Tenant("Audit Test", "audit-test");
        dbContext.Tenants.Add(tenant);
        await dbContext.SaveChangesAsync();

        // Assert
        var auditLog = await dbContext.AuditLogs
            .FirstOrDefaultAsync(a => a.EntityId == tenant.Id && a.Action == "Insert");

        auditLog.Should().NotBeNull();
        auditLog!.EntityName.Should().Be(nameof(Tenant));
        tenant.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
    }

    [Fact]
    public async Task Should_CreateAuditLog_With_CorrectValues_When_EntityIsUpdated()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IamPlatformDbContext>();

        var tenant = new Tenant("Original Name", "original-slug");
        dbContext.Tenants.Add(tenant);
        await dbContext.SaveChangesAsync();

        // Act
        tenant.UpdateName("New Name");
        await dbContext.SaveChangesAsync();

        // Assert
        var auditLog = await dbContext.AuditLogs
            .Where(a => a.EntityId == tenant.Id && a.Action == "Update")
            .OrderByDescending(a => a.Timestamp)
            .FirstOrDefaultAsync();

        auditLog.Should().NotBeNull();
        auditLog!.Changes.Should().Contain("New Name");
        auditLog.Changes.Should().Contain("Original Name");
        tenant.UpdatedAt.Should().NotBeNull();
    }
}
