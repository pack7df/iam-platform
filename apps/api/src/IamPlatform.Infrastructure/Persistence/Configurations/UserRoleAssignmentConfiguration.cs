using IamPlatform.Domain.Tenants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IamPlatform.Infrastructure.Persistence.Configurations;

public sealed class UserRoleAssignmentConfiguration : IEntityTypeConfiguration<UserRoleAssignment>
{
    public void Configure(EntityTypeBuilder<UserRoleAssignment> builder)
    {
        builder.HasKey(ura => ura.Id);
        
        builder.Property(ura => ura.Id)
            .HasMaxLength(50)
            .IsRequired();
            
        builder.Property(ura => ura.UserId)
            .HasMaxLength(50)
            .IsRequired();
            
        builder.Property(ura => ura.RoleId)
            .HasMaxLength(50)
            .IsRequired();
            
        // Shadow property for AssignedAt (not in domain entity)
        builder.Property<DateTime>("AssignedAt")
            .IsRequired()
            .HasDefaultValueSql("NOW()");
            
        // Relationships
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(ura => ura.UserId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne<Role>()
            .WithMany()
            .HasForeignKey(ura => ura.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Unique constraint on (user_id, role_id) - but entity has Id as PK
        // For uniqueness, we could add unique index on UserId+RoleId if needed
        // However, the entity uses Id as PK, and the domain ensures no duplicates via business logic
        // Skipping unique DB constraint for now (can be added later)
        
        builder.HasIndex(ura => ura.UserId);
        builder.HasIndex(ura => ura.RoleId);
    }
}
