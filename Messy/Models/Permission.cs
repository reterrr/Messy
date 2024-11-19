using System.ComponentModel.DataAnnotations;
using Messy.Contexts.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Messy.Models;
[EntityTypeConfiguration(typeof(PermissionConfiguration))]
public class Permission
{
    [Key]
    public long Id { get; set; }
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }
    
    public string Slug { get; set; }
    
    public List<Role> Roles { get; set; }
    
    public List<ChatUser> ChatUsers { get; set; }
}