namespace Messy.Requests;

public class DeleteMessageRequest : Request
{
    public long ChatId { get; set; }
    public long MessageId { get; set; }
}