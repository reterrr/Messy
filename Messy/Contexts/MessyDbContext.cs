using Messy.Actions;
using Messy.Models;
using Microsoft.EntityFrameworkCore;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;
using File = Messy.Models.File;

namespace Messy.Contexts;

public class MessyDbContext : DbContext
{
    public MessyDbContext(DbContextOptions<MessyDbContext> options)
    : base(options)
    {
    }

    public DbSet<User> User { get; set; }
    public DbSet<UserProfile> UserProfile { get; set; }
    public DbSet<Permission> Permission { get; set; }
    public DbSet<Chat> Chat { get; set; }
    public DbSet<File> File { get; set; }
    public DbSet<Role> Role { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<ChatUser> ChatUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}