using System.ComponentModel.DataAnnotations;

namespace Messy.Requests;

public class SearchUserRequest : Request
{
    [MaxLength(255, ErrorMessage = "User name cannot be longer than 255 characters.")]
    public string Username { get; set; }
}