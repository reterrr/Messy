using System.ComponentModel.DataAnnotations;
using Messy.Contexts.Configurations;
using Messy.Helpers.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Messy.Models;
[EntityTypeConfiguration(typeof(ChatConfiguration))]
public class Chat : ITimeStampable, ISoftDeletable
{
    [Key]
    public long Id { get; set; }
    [MaxLength(50)]
    public string? Title { get; set; }
    
    [Required]
    public ChatType Type { get; set; }

    [Required]
    public List<User> Users { get; set; }
    [Required]
    public List<ChatUser> ChatUsers { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}