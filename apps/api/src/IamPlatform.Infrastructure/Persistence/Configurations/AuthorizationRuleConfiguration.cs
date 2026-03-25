using IamPlatform.Domain.Authorization;
using IamPlatform.Domain.Tenants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IamPlatform.Infrastructure.Persistence.Configurations;

public sealed class AuthorizationRuleConfiguration : IEntityTypeConfiguration<AuthorizationRule>
{
    public void Configure(EntityTypeBuilder<AuthorizationRule> builder)
    {
        builder.HasKey(ar => ar.Id);
        
        builder.Property(ar => ar.Id)
            .HasMaxLength(50)
            .IsRequired();
            
        builder.Property(ar => ar.ResourceId)
            .HasMaxLength(50)
            .IsRequired();
            
        builder.Property(ar => ar.OperationId)
            .HasMaxLength(50)
            .IsRequired();
            
        builder.Property(ar => ar.UserId)
            .HasMaxLength(50);
            
        builder.Property(ar => ar.RoleId)
            .HasMaxLength(50);
            
        builder.Property(ar => ar.Decision)
            .IsRequired()
            .HasConversion<string>();
            
        builder.Property(ar => ar.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
            
        // Shadow properties for timestamps
        builder.Property<DateTime>("CreatedAt")
            .IsRequired()
            .HasDefaultValueSql("NOW()");
            
        builder.Property<DateTime>("UpdatedAt")
            .IsRequired()
            .HasDefaultValueSql("NOW()");
            
        // Relationships
        builder.HasOne<Resource>()
            .WithMany()
            .HasForeignKey(ar => ar.ResourceId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne<Operation>()
            .WithMany()
            .HasForeignKey(ar => ar.OperationId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(ar => ar.UserId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne<Role>()
            .WithMany()
            .HasForeignKey(ar => ar.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Indexes
        builder.HasIndex(ar => ar.ResourceId);
        builder.HasIndex(ar => ar.OperationId);
        builder.HasIndex(ar => ar.UserId).HasFilter("[user_id] IS NOT NULL");
        builder.HasIndex(ar => ar.RoleId).HasFilter("[role_id] IS NOT NULL");
        builder.HasIndex(ar => ar.IsActive);
    }
}
