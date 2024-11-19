using Messy.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messy.Contexts.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.UserName)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(u => u.UserName)
            .IsUnique();

        builder.HasOne(u => u.UserProfile)
            .WithOne(up => up.User)
            .HasForeignKey<UserProfile>(p => p.Id)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany<Chat>(u => u.Chats)
            .WithMany(c => c.Users)
            .UsingEntity<ChatUser>
            (
                "ChatUsers",
                l => l
                    .HasOne<Chat>(cu => cu.Chat)
                    .WithMany(c => c.ChatUsers)
                    .HasForeignKey(cu => cu.ChatId)
                    .HasPrincipalKey(nameof(Chat.Id))
                    .OnDelete(DeleteBehavior.Cascade),
                r => r.HasOne<User>(cu => cu.User)
                    .WithMany(u => u.ChatUsers)
                    .HasForeignKey(cu => cu.UserId)
                    .HasPrincipalKey(nameof(User.Id))
                    .OnDelete(DeleteBehavior.Cascade)
            );
    }
}