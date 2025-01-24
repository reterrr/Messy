namespace Messy.Requests;

public class CreateChatRequest : Request
{
    public ChatType chatType { get; set; }
}