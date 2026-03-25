using IamPlatform.Domain.Authorization;
using IamPlatform.Domain.Tenants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

// Alias to avoid namespace conflict
using ApplicationEntity = IamPlatform.Domain.Tenants.Application;

namespace IamPlatform.Infrastructure.Persistence.Configurations;

public sealed class ResourceConfiguration : IEntityTypeConfiguration<Resource>
{
    public void Configure(EntityTypeBuilder<Resource> builder)
    {
        builder.HasKey(r => r.Id);
        
        builder.Property(r => r.Id)
            .HasMaxLength(50)
            .IsRequired();
            
        builder.Property(r => r.ApplicationId)
            .HasMaxLength(50)
            .IsRequired();
            
        builder.Property(r => r.Name)
            .HasMaxLength(255)
            .IsRequired();
            
        builder.Property(r => r.Key)
            .HasMaxLength(100)
            .IsRequired();
            
        builder.Property(r => r.ParentId)
            .HasMaxLength(50);
            
        builder.Property(r => r.IsActive)
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
        builder.HasOne<ApplicationEntity>()
            .WithMany()
            .HasForeignKey(r => r.ApplicationId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne<Resource>()
            .WithMany()
            .HasForeignKey(r => r.ParentId)
            .OnDelete(DeleteBehavior.Restrict);
            
        // Indexes
        builder.HasIndex(r => r.ApplicationId);
        builder.HasIndex(r => r.ParentId);
        builder.HasIndex(r => r.IsActive);
    }
}
