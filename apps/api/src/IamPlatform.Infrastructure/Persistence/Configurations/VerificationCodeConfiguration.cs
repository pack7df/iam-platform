using IamPlatform.Domain.Tenants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IamPlatform.Infrastructure.Persistence.Configurations;

public sealed class VerificationCodeConfiguration : IEntityTypeConfiguration<VerificationCode>
{
    public void Configure(EntityTypeBuilder<VerificationCode> builder)
    {
        builder.HasKey(vc => vc.Id);

        builder.Property(vc => vc.Id)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(vc => vc.UserId)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(vc => vc.Code)
            .HasMaxLength(6)
            .IsRequired();

        builder.Property(vc => vc.Type)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(vc => vc.ExpiresAt)
            .IsRequired();

        builder.Property(vc => vc.IsUsed)
            .IsRequired()
            .HasDefaultValue(false);

        // Security: Index by code and userId for fast lookups
        builder.HasIndex(vc => new { vc.UserId, vc.Code, vc.IsUsed });
        
        // Relationship (optional but good for consistency)
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(vc => vc.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
