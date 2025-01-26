using Messy.Actions.Chat;

namespace Messy.Requests;

public class CreateOneToOneRequest : CreateChatRequest
{
    public long UserId { get; set; }

    public CreateOneToOneRequest()
    {
        Type = (short)ChatType.OneToOne;
    }
}