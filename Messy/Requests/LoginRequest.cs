using System.ComponentModel.DataAnnotations;

namespace Messy.Requests;

public class LoginRequest : Request
{
    [Required(ErrorMessage = "Username is required")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; }
}