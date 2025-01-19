using Messy.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Messy.Helpers;

public class AuthValidator
{
    public User? ExtractValidUser(string username, string password)
    {
        User? user = null;

        using (var connection = NpgslqConnector.CreateConnection())
        {
            connection.Open();
            var getUserCommand = new NpgsqlCommand("SELECT * FROM users WHERE username = @username", connection);
            getUserCommand.Parameters.AddWithValue("@username", username);

            using (var reader = getUserCommand.ExecuteReader())
            {
                if (reader.Read())
                {
                    user = new User
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("id")),
                        UserName = reader.GetString(reader.GetOrdinal("username")),
                        Password = reader.GetString(reader.GetOrdinal("password"))
                    };
                }
            }
        }

        if (user != null &&
            PasswordHasher.Verify(password, user.Password)) return user;

        Message = "Invalid username or password";

        return null;
    }

    public bool UserExists(string matching)
    {
        var result = false;
        using var connection = NpgslqConnector.CreateConnection();
        connection.Open();
        var userExistsCommand =
            new NpgsqlCommand("SELECT EXISTS (SELECT 1 FROM users WHERE username = @matching)", connection);
        userExistsCommand.Parameters.AddWithValue("@matching", matching);

        using var reader = userExistsCommand.ExecuteReader();
        if (reader.Read())
        {
            result = reader.GetBoolean(0);
        }

        return result;
    }

    public static bool UserExists(long userId)
    {
        using var connection = NpgslqConnector.CreateConnection();

        connection.Open();

        var userExistsCommand = new NpgsqlCommand("select exists(select 1 from users where id = @userId) ", connection);
        userExistsCommand.Parameters.AddWithValue("userId", userId);
        
        
        return (bool)userExistsCommand.ExecuteScalar();
    }

    public static AuthValidator Make()
    {
        return new AuthValidator();
    }

    public static string? Message;
}