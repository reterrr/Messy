using System.ComponentModel.DataAnnotations;

namespace Messy.Requests;

public class CreateManyToManyRequest : CreateChatRequest
{
    [Required]
    public List<long> UserIds { get; set; }

    public CreateManyToManyRequest()
    {
        Type = (short)ChatType.ManyToMany;
    }
}