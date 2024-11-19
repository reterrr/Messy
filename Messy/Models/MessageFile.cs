using Messy.Helpers;

namespace Messy.Models;

public class MessageFile
{
    public List<Message> Messages { get; set; }
    
    [ValidateFile]
    public List<File> Files { get; set; }
}