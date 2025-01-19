using Npgsql;

namespace Messy.Helpers;

public static class NpgslqConnector
{
    public static void setConnectionString(string connectionString)
    {
        connectionString_ = connectionString;
    }

    public static NpgsqlConnection CreateConnection()
    {
        return new NpgsqlConnection(connectionString_);
    }

    private static string connectionString_;
}