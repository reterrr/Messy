using Messy.Helpers;
using Messy.Requests;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Messy.Actions.User;

public class DeleteUserAction(DeleteUserRequest request) : IAction<DeleteUserRequest>
{
    public static IAction<DeleteUserRequest> Make(DeleteUserRequest request)
    {
        return new DeleteUserAction(request);
    }

    public IActionResult Execute()
    {
        var userId = Request.GetCurrentUserId();

        using (var connection = NpgslqConnector.CreateConnection())
        {
            connection.Open();
            var deleteUserCommand = new NpgsqlCommand("select delete_user(@user_id)", connection);
            deleteUserCommand.Parameters.AddWithValue("user_id", userId);

            using var reader = deleteUserCommand.ExecuteReader();

            if (reader.Read())
            {
                var isDeleted = reader.GetBoolean(0);

                if (!isDeleted)
                {
                    return new NotFoundObjectResult("User not found");
                }
            }
        }

        return new OkObjectResult("deleted successfully");
    }

    public DeleteUserRequest Request { get; } = request;
}