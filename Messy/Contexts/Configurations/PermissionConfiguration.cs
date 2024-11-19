using Messy.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messy.Contexts.Configurations;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .HasMaxLength(80)
            .IsRequired();
        
        builder.Property(p => p.Slug)
            .HasMaxLength(50)
            .IsRequired();
        
        builder.HasMany<Role>(p => p.Roles)
            .WithMany(r => r.Permissions)
            .UsingEntity("RolesPermissions");
        
        builder.HasIndex(p => p.Slug)
            .IsUnique();
        
    }
}