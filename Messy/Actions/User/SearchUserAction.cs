using Messy.Helpers;
using Messy.Requests;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Messy.Actions.User;

public class SearchUserAction(SearchUserRequest request) : IAction<SearchUserRequest>
{
    public static IAction<SearchUserRequest> Make(SearchUserRequest request)
    {
        return new SearchUserAction(request);
    }

    public IActionResult Execute()
    {
        using var connection = NpgslqConnector.CreateConnection();
        connection.Open();

        var searchUsersCommand = new NpgsqlCommand("select * from search_users(@username)", connection);
        searchUsersCommand.Parameters.AddWithValue("username", Request.Username);

        using var reader = searchUsersCommand.ExecuteReader();
        var users = new List<Models.User>();

        while (reader.Read())
        {
            users.Add(new Models.User
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
            });
        }

        return new OkObjectResult(users);
    }

    public SearchUserRequest Request { get; } = request;
}