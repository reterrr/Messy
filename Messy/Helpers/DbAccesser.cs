using Messy.Contexts;

namespace Messy.Helpers;

public class DbAccesser
{
    public static MessyDbContext Context;

    public DbAccesser(MessyDbContext context)
    {
        Context = context;
    }
}