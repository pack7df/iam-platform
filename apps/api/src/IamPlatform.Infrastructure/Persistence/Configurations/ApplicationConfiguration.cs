using IamPlatform.Domain.Tenants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IamPlatform.Infrastructure.Persistence.Configurations;

public sealed class ApplicationConfiguration : IEntityTypeConfiguration<IamPlatform.Domain.Tenants.Application>
{
    public void Configure(EntityTypeBuilder<IamPlatform.Domain.Tenants.Application> builder)
    {
        builder.HasKey(a => a.Id);
        
        builder.Property(a => a.Id)
            .HasMaxLength(50)
            .IsRequired();
            
        builder.Property(a => a.TenantId)
            .HasMaxLength(50)
            .IsRequired();
            
        builder.Property(a => a.Name)
            .HasMaxLength(255)
            .IsRequired();
            
        builder.Property(a => a.IsActive)
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
        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(a => a.TenantId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasIndex(a => a.TenantId);
    }
}
