using System.ComponentModel.DataAnnotations;
using Messy.Contexts.Configurations;
using Messy.Helpers.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Messy.Models;
[EntityTypeConfiguration(typeof(UserProfileConfiguration))]
public class UserProfile : ITimeStampable, ISoftDeletable
{
    [Key]
    public long Id { get; set; }
    [MaxLength(500)]
    public string  Description { get; set; }
    
    [Required]
    public long UserId { get; set; }
    [Required]
    public User User { get; set; }

    public List<File> Images { get; } = [];
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}