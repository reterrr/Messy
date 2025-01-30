using Messy.Helpers;
using Messy.Requests;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Messy.Actions.User;

public class GetUserAction(GetUserRequest request) : IAction<GetUserRequest>
{
    public static IAction<GetUserRequest> Make(GetUserRequest request)
    {
        return new GetUserAction(request);
    }

    public IActionResult Execute()
    {
        using var connection = NpgsqlConnector.CreateConnection();
        connection.Open();
        var getUserCommand = new NpgsqlCommand("select * from users where id = @user", connection);
        getUserCommand.Parameters.AddWithValue("user", Request.UserId);

        using var reader = getUserCommand.ExecuteReader();
        Models.User? user = null;

        if (reader.Read())
        {
            user = new Models.User
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Name = reader.GetString(reader.GetOrdinal("name")),
                UserName = reader.GetString(reader.GetOrdinal("username")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("createdat")),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("updatedat"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("updatedat")),
                DeletedAt = reader.IsDBNull(reader.GetOrdinal("deletedat"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("deletedat")),
            };
        }

        return new OkObjectResult(user);
    }

    public GetUserRequest Request { get; } = request;
}