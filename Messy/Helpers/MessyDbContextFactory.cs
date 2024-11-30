using Messy.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Messy.Helpers;

public static class MessyDbContextFactory
{
    private static string ConnectionString;

    public static void SetConnectionString(string connectionString)
    {
        ConnectionString = connectionString;
    }

    public static MessyDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<MessyDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        return new MessyDbContext(options);
    }
}