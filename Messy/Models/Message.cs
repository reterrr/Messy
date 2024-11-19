using System.ComponentModel.DataAnnotations;
using Messy.Contexts.Configurations;
using Messy.Helpers;
using Messy.Helpers.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Messy.Models;
[EntityTypeConfiguration(typeof(MessageConfiguration))]
public class Message : ITimeStampable, ISoftDeletable
{
    [Key]
    public long Id { get; set; }
    [Required]
    [MaxLength(5000)]
    public string Body { get; set; }
    
    public long ParentId { get; set; }
    public Message? ParentMessage { get; set; }
    
    public List<Message> Replies { get; set; }
    
    [ValidateFile]
    public List<File> Files { get; set; } = [];
    
    [Required]
    public long ChatUserId { get; set; }
    [Required]
    public ChatUser ChatUser { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}