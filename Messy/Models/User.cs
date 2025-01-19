using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Messy.Helpers.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace Messy.Models;
[Keyless]
public class User : ITimeStampable, ISoftDeletable
{
    public long Id { get; set; }
    [Required]
    [MaxLength(255)]
    public string Name { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string UserName { get; set; }

    [Required] 
    [NotMapped]
    public string Password { get; set; }
    
    [Required]
    public long UserProfileId { get; set; }
    [Required]
    public UserProfile UserProfile { get; set; }
    
    public List<Chat> Chats { get; set; } = [];
    public List<ChatUser> ChatUsers { get; set; } = [];
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}