using System.ComponentModel.DataAnnotations;
using Messy.Contexts.Configurations;
using Messy.Helpers.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Messy.Models;
[EntityTypeConfiguration(typeof(FileConfiguration))]
public class File : ITimeStampable, ISoftDeletable
{
    [Key]
    public long Id { get; set; }
    [Required]
    [MaxLength(1024)]
    public string Path { get; set; }
    [Required]
    public byte[] Data { get; set; }
    [Required]
    public long Length { get; set; } 
    public long? UserProfileId { get; set; }
    public UserProfile? UserProfile { get; set; }
    
    public List<Message> Messages { get; set; }
    
    [Required] 
    public virtual FileType Type { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}