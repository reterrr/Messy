namespace Messy.Requests;

public class UpdateMessageRequest : Request
{
    public long ChatId { get; set; }
    public long MessageId { get; set; }
    public string NewBody { get; set; }
}