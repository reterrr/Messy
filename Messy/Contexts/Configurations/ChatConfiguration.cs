using Messy.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Messy.Contexts.Configurations;

public class ChatConfiguration : CreatableConfiguration<Chat>
{
    public new void Configure(EntityTypeBuilder<Chat> builder)
    {
        base.Configure(builder);

        builder.ToTable("Chats");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Title)
            .HasDefaultValue(null)
            .HasMaxLength(100);

        builder.Property(c => c.Type)
            .HasConversion<int>();
    }
}