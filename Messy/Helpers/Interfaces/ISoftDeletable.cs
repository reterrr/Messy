
namespace Messy.Helpers.Interfaces;

public interface ISoftDeletable
{
    public DateTime? DeletedAt { get; set; }

    public sealed bool IsDeleted()
    {
        return DeletedAt != null;
    }
}