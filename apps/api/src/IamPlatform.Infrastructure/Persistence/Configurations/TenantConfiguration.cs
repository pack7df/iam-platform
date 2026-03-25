using IamPlatform.Domain.Tenants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IamPlatform.Infrastructure.Persistence.Configurations;

public sealed class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.HasKey(t => t.Id);
        
        builder.Property(t => t.Id)
            .HasMaxLength(50)
            .IsRequired();
            
        builder.Property(t => t.Name)
            .HasMaxLength(255)
            .IsRequired();
            
        builder.Property(t => t.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
            
        // Shadow properties for timestamps
        builder.Property<DateTime>("CreatedAt")
            .IsRequired()
            .HasDefaultValueSql("NOW()");
            
        builder.Property<DateTime>("UpdatedAt")
            .IsRequired()
            .HasDefaultValueSql("NOW()");
    }
}
