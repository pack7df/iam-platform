using IamPlatform.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IamPlatform.Infrastructure.Persistence.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.EntityName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Action)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(e => e.Changes)
            .HasColumnType("text");

        builder.HasIndex(e => e.EntityId);
        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => e.Timestamp);
    }
}
