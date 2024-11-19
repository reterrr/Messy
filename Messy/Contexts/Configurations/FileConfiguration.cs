using Messy.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using File = Messy.Models.File;

namespace Messy.Contexts.Configurations;

public class FileConfiguration : CreatableConfiguration<File>
{
    public new void Configure(EntityTypeBuilder<File> builder)
    {
        base.Configure(builder);

        builder.ToTable("Files");

        builder.HasKey(f => f.Id);
        
        builder.Property(f => f.Path)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(f => f.Length)
            .IsRequired();

        builder.Property(f => f.Data)
            .HasColumnType("BYTEA");
        
        builder.Property(f => f.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.HasOne<UserProfile>(f => f.UserProfile)
            .WithMany(up => up.Images)
            .HasForeignKey(f => f.UserProfileId);
    }
}