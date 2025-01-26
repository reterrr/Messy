using Messy.Helpers;

namespace Messy.Requests;

public class AddGroupUserRequest : Request
{
    [ExcludeDuplicates]
    public List<long> UserIds { get; set; }
    
    public long ChatId { get; set; }
}