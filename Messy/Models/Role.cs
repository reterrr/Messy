using System.ComponentModel.DataAnnotations;

using Microsoft.EntityFrameworkCore;

namespace Messy.Models;
public class Role
{
    [Key]
    public long Id { get; set; }
    public byte Priority { get; set; }
    [Required]
    [MaxLength(70)]
    public string Name { get; set; }
    [Required]
    [MaxLength(50)]
    public string Slug { get; set; }
    
    public List<Permission> Permissions { get; set; } = [];
    public List<ChatUser> ChatUsers { get; set; } = [];
}