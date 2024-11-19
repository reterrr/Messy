using Messy.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messy.Contexts.Configurations;

public class ChatUserConfguration : CreatableConfiguration<ChatUser>
{
    public new void Configure(EntityTypeBuilder<ChatUser> builder)
    {
        base.Configure(builder);

        builder.ToTable("ChatUsers");
        builder.HasKey(cu => new { cu.ChatId, cu.UserId });

        builder.HasMany<Message>(cu => cu.Messages)
            .WithOne(m => m.ChatUser)
            .HasForeignKey(m => m.ChatUserId);

        builder.HasMany<Role>(cu => cu.Roles)
            .WithMany(r => r.ChatUsers)
            .UsingEntity("ChatUsersRoles");

        builder.HasMany<Permission>(cu => cu.Permissions)
            .WithMany(p => p.ChatUsers)
            .UsingEntity("ChatUsersPermissions");
    }
}