using System.ComponentModel.DataAnnotations;

namespace Messy.Requests;

public class CreateMessageRequest : Request
{
    [MaxLength(5000)]
    [Required(AllowEmptyStrings = false)]
    public string Body { get; set; }

    public long ParentId { get; set; } = 0;
    
    public long ChatId { get; set; }

    public List<IFormFile> Files { get; set; }
}