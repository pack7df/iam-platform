using IamPlatform.Domain.Authorization;
using IamPlatform.Domain.Common;
using IamPlatform.Domain.Identity;
using IamPlatform.Domain.Tenants;
using Microsoft.EntityFrameworkCore;

// Alias only for Application to avoid conflict with IamPlatform.Application namespace
using ApplicationEntity = IamPlatform.Domain.Tenants.Application;

namespace IamPlatform.Infrastructure.Persistence;

public sealed class IamPlatformDbContext : DbContext, IUnitOfWork
{
    public IamPlatformDbContext(DbContextOptions<IamPlatformDbContext> options)
        : base(options)
    {
    }

    // Tenants
    public DbSet<Tenant> Tenants => Set<Tenant>();
    
    // Users & Identity
    public DbSet<User> Users => Set<User>();
    public DbSet<Invitation> Invitations => Set<Invitation>();
    public DbSet<VerificationCode> VerificationCodes => Set<VerificationCode>();
    
    // Roles & Assignments
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRoleAssignment> UserRoleAssignments => Set<UserRoleAssignment>();
    
    // Applications - use alias to avoid namespace conflict
    public DbSet<ApplicationEntity> Applications => Set<ApplicationEntity>();
    
    // Authorization
    public DbSet<Resource> Resources => Set<Resource>();
    public DbSet<Operation> Operations => Set<Operation>();
    public DbSet<AuthorizationRule> AuthorizationRules => Set<AuthorizationRule>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply all configurations from the Configurations folder
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IamPlatformDbContext).Assembly);
    }
}
