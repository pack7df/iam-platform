using IamPlatform.Domain.Tenants;
using IamPlatform.Domain.Users;
using IamPlatform.Domain.Applications;
using Microsoft.EntityFrameworkCore;

namespace IamPlatform.Infrastructure.Persistence;

public class IamPlatformDbContext : DbContext
{
    public IamPlatformDbContext(DbContextOptions<IamPlatformDbContext> options) : base(options)
    {
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<User> Users => Set<User>();
    public DbSet<IamPlatform.Domain.Applications.Application> Applications => Set<IamPlatform.Domain.Applications.Application>();
    public DbSet<Resource> Resources => Set<Resource>();
    public DbSet<IamPlatform.Domain.Operations.Operation> Operations => Set<IamPlatform.Domain.Operations.Operation>();
    public DbSet<IamPlatform.Domain.Applications.Action> Actions => Set<IamPlatform.Domain.Applications.Action>();

    public DbSet<IamPlatform.Domain.Tenants.Role> Roles => Set<IamPlatform.Domain.Tenants.Role>();
    public DbSet<IamPlatform.Domain.Authorization.Permission> Permissions => Set<IamPlatform.Domain.Authorization.Permission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Slug).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Slug).IsUnique();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PasswordHash).IsRequired();
            
            entity.HasIndex(e => new { e.TenantId, e.Email }).IsUnique();

            entity.HasOne<Tenant>()
                .WithMany()
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<IamPlatform.Domain.Tenants.Role>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            
            entity.HasIndex(e => new { e.TenantId, e.Name }).IsUnique();

            entity.HasOne<Tenant>()
                .WithMany()
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<IamPlatform.Domain.Applications.Application>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Slug).IsRequired().HasMaxLength(100);
            
            entity.HasIndex(e => new { e.TenantId, e.Slug }).IsUnique();

            entity.HasOne<Tenant>()
                .WithMany()
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Resource>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Key).IsRequired().HasMaxLength(255);
            
            entity.HasIndex(e => new { e.ApplicationId, e.Key }).IsUnique();

            entity.HasOne<IamPlatform.Domain.Applications.Application>()
                .WithMany()
                .HasForeignKey(e => e.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne<Resource>()
                .WithMany()
                .HasForeignKey(e => e.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<IamPlatform.Domain.Operations.Operation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Key).IsRequired().HasMaxLength(100);

            entity.HasIndex(e => new { e.TenantId, e.Key }).IsUnique();

            entity.HasOne<Tenant>()
                .WithMany()
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<IamPlatform.Domain.Applications.Action>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => new { e.ResourceId, e.OperationId }).IsUnique();

            entity.HasOne<Resource>()
                .WithMany()
                .HasForeignKey(e => e.ResourceId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne<IamPlatform.Domain.Operations.Operation>()
                .WithMany()
                .HasForeignKey(e => e.OperationId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<IamPlatform.Domain.Authorization.Permission>(entity =>
        {
            entity.HasKey(e => e.Id);

            // Constraint: A User can only have one explicit permission per Action
            entity.HasIndex(e => new { e.ActionId, e.UserId }).IsUnique().HasFilter("\"UserId\" IS NOT NULL");
            // Constraint: A Role can only have one explicit permission per Action
            entity.HasIndex(e => new { e.ActionId, e.RoleId }).IsUnique().HasFilter("\"RoleId\" IS NOT NULL");

            entity.HasOne<IamPlatform.Domain.Applications.Action>()
                .WithMany()
                .HasForeignKey(e => e.ActionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne<IamPlatform.Domain.Tenants.Role>()
                .WithMany()
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}

