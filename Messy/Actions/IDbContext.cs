using Messy.Contexts;

namespace Messy.Actions;

public abstract class DbContext
{
    protected static MessyDbContext Context;

    protected DbContext(MessyDbContext context)
    {
        Context = context;
    }
}