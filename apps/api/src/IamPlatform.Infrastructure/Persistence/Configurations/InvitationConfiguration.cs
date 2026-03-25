using IamPlatform.Domain.Identity;
using IamPlatform.Domain.Tenants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IamPlatform.Infrastructure.Persistence.Configurations;

public sealed class InvitationConfiguration : IEntityTypeConfiguration<Invitation>
{
    public void Configure(EntityTypeBuilder<Invitation> builder)
    {
        builder.HasKey(i => i.Id);
        
        builder.Property(i => i.Id)
            .HasMaxLength(50)
            .IsRequired();
            
        builder.Property(i => i.InvitedIdentityId)
            .HasMaxLength(50)
            .IsRequired();
            
        builder.Property(i => i.TargetType)
            .IsRequired()
            .HasConversion<string>();
            
        builder.Property(i => i.TenantId)
            .HasMaxLength(50);
            
        builder.Property(i => i.Status)
            .IsRequired()
            .HasConversion<string>();
            
        // Shadow properties for timestamps
        builder.Property<DateTime>("CreatedAt")
            .IsRequired()
            .HasDefaultValueSql("NOW()");
            
        builder.Property<DateTime>("UpdatedAt")
            .IsRequired()
            .HasDefaultValueSql("NOW()");
            
        // Shadow properties for invitation-specific dates (not in domain entity)
        builder.Property<DateTime>("ExpiresAt")
            .IsRequired()
            .HasDefaultValueSql("NOW() + INTERVAL '7 days'");
            
        builder.Property<DateTime?>("AcceptedAt");
            
        // Relationships
        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(i => i.TenantId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Indexes
        builder.HasIndex(i => i.TenantId);
        builder.HasIndex(i => i.InvitedIdentityId);
        builder.HasIndex(i => i.Status);
    }
}
