using IamPlatform.Domain.Authorization;
using IamPlatform.Domain.Tenants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

// Alias to avoid namespace conflict
using ApplicationEntity = IamPlatform.Domain.Tenants.Application;

namespace IamPlatform.Infrastructure.Persistence.Configurations;

public sealed class OperationConfiguration : IEntityTypeConfiguration<Operation>
{
    public void Configure(EntityTypeBuilder<Operation> builder)
    {
        builder.HasKey(o => o.Id);
        
        builder.Property(o => o.Id)
            .HasMaxLength(50)
            .IsRequired();
            
        builder.Property(o => o.ApplicationId)
            .HasMaxLength(50)
            .IsRequired();
            
        builder.Property(o => o.Key)
            .HasMaxLength(100)
            .IsRequired();
            
        builder.Property(o => o.Name)
            .HasMaxLength(255)
            .IsRequired();
            
        builder.Property(o => o.IsActive)
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
            .HasForeignKey(o => o.ApplicationId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Indexes
        builder.HasIndex(o => o.ApplicationId);
        builder.HasIndex(o => o.IsActive);
    }
}
