using System.ComponentModel.DataAnnotations;

namespace Messy.Requests;

public class CreateOneToOneRequest : CreateChatRequest
{
    [Required]
    public long UserId { get; set; }

    public CreateOneToOneRequest()
    {
        Type = (short)ChatType.OneToOne;
    }
}