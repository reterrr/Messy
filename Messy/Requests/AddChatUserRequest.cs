using Messy.Helpers;

namespace Messy.Requests;

public class AddChatUserRequest : Request
{
    public long userId { get; set; }
    
    public long chatId { get; set; }
}