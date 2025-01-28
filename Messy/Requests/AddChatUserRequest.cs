using System.ComponentModel.DataAnnotations;

namespace Messy.Requests;

public class AddChatUserRequest : Request
{
    [Required]
    public long userId { get; set; }
    
    public long chatId { get; set; }
}