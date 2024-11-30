using System.ComponentModel.DataAnnotations;

namespace Messy.Requests;

public class RegisterRequest : Request
{
    [Required(ErrorMessage = "Username is required")]
    public string UserName { get; set; }
    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; }
    [Required(ErrorMessage = "Check Password is required")]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string CheckPassword { get; set; }
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }
}