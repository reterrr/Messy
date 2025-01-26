namespace Messy.Requests;

public class CreateManyToManyRequest : CreateChatRequest
{
    public List<long> UserIds { get; set; }

    public CreateManyToManyRequest()
    {
        Type = (short)ChatType.ManyToMany;
    }
}