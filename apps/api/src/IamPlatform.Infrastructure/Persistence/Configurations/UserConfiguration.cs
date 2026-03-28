using IamPlatform.Domain.Tenants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IamPlatform.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.Id)
            .HasMaxLength(50)
            .IsRequired();
            
        builder.Property(u => u.TenantId)
            .HasMaxLength(50); // Nullable: SystemUser has null tenant_id
            
        builder.Property(u => u.Type)
            .HasMaxLength(30)
            .HasConversion<string>()
            .IsRequired();
            
        builder.Property(u => u.Status)
            .HasMaxLength(30)
            .HasConversion<string>()
            .IsRequired()
            .HasDefaultValue(UserStatus.PendingVerification);

        builder.Property(u => u.PasswordHash)
            .HasMaxLength(500);

        builder.Property(u => u.Salt)
            .HasMaxLength(100);
            
        // Shadow properties for timestamps
        builder.Property<DateTime>("CreatedAt")
            .IsRequired()
            .HasDefaultValueSql("NOW()");
            
        builder.Property<DateTime>("UpdatedAt")
            .IsRequired()
            .HasDefaultValueSql("NOW()");
            
        // Index for tenant lookups (only for TenantUsers, where tenant_id is not null)
        builder.HasIndex(u => u.TenantId).HasFilter("\"TenantId\" IS NOT NULL");
        builder.HasIndex(u => u.Type);
    }
}
