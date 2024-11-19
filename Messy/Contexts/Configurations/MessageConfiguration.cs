using Messy.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using File = Messy.Models.File;

namespace Messy.Contexts.Configurations;

public class MessageConfiguration : CreatableConfiguration<Message>
{
    public new void Configure(EntityTypeBuilder<Message> builder)
    {
        base.Configure(builder);
        
        builder.ToTable("Messages");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Body)
            .HasColumnType("varchar(5000)");

        builder.HasMany<File>(m => m.Files)
            .WithMany(f => f.Messages)
            .UsingEntity("FilesMessages");
        
        builder.HasOne<Message>(m => m.ParentMessage)
            .WithMany(m => m.Replies)
            .HasForeignKey(m => m.ParentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}