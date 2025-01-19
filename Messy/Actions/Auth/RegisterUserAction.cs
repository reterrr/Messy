using Messy.Helpers;
using Messy.Requests;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Messy.Actions.Auth;

public class RegisterUserAction(RegisterRequest request) : IAction<RegisterRequest>
{
    public static IAction<RegisterRequest> Make(RegisterRequest request)
    {
        return new RegisterUserAction(request);
    }

    public IActionResult Execute()
    {
        if (AuthValidator.Make().UserExists(Request.UserName))
            return new BadRequestObjectResult("Such user already exists.");

        try
        {
            using (var connection = NpgslqConnector.CreateConnection())
            {
                connection.Open();
                var insetUser =
                    new NpgsqlCommand(
                        "insert into users(username,name, password, createdat) values (@username, @name, @password, (SELECT NOW())) returning id",
                        connection);
                insetUser.Parameters.AddWithValue("userName", Request.UserName);
                insetUser.Parameters.AddWithValue("password", PasswordHasher.Hash(Request.Password));
                insetUser.Parameters.AddWithValue("name", Request.Name);

                var userId = 0;

                using (var reader = insetUser.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        userId = reader.GetInt32(reader.GetOrdinal("id"));
                    }
                }

                var insertUserProfile =
                    new NpgsqlCommand(
                        "insert into userprofiles(userid, createdat) values (@userid, (SELECT NOW()))",
                        connection);
                insertUserProfile.Parameters.AddWithValue("userId", userId);
                insertUserProfile.Parameters.AddWithValue("createdat", DateTime.UtcNow);

                insertUserProfile.ExecuteNonQuery();
            }

            return new OkObjectResult("Registered Successfully");
        }
        catch (Exception ex)
        {
            return new ObjectResult($"Error occurred: {ex.Message}") { StatusCode = 500 };
        }
    }
    
    public RegisterRequest Request { get; } = request;
}