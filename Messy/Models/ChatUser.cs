using Messy.Helpers.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Messy.Models;
public class ChatUser : ITimeStampable, ISoftDeletable
{
    public long UserId { get; set; }
    public User User { get; set; }
    public long ChatId { get; set; }
    public Chat Chat { get; set; }

    public List<Message> Messages { get; set; } = [];
    public List<Role> Roles { get; set; } = [];
    public List<Permission> Permissions { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}