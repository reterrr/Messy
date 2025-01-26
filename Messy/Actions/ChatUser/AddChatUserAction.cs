using Messy.Helpers;
using Messy.Requests;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Messy.Actions.ChatUser;

public class AddChatUserAction(AddChatUserRequest request) : IAction<AddChatUserRequest>
{
    public static IAction<AddChatUserRequest> Make(AddChatUserRequest request)
    {
        return new AddChatUserAction(request);
    }

    public IActionResult Execute()
    {
        var chatId = Request.chatId;
        var userId = Request.userId;

        using var connection = NpgslqConnector.CreateConnection();
        connection.Open();

        var chatExistsCommand = new NpgsqlCommand(
            "SELECT 1 FROM chats WHERE id = @chatId",
            connection
        );

        chatExistsCommand.Parameters.AddWithValue("chatId", chatId);

        var chatExists = chatExistsCommand.ExecuteScalar() != null;
        if (!chatExists)
        {
            return new BadRequestObjectResult(new { error = $"Chat with id {chatId} does not exist." });
        }

        var usersExistCommand = new NpgsqlCommand(
            $"select EXISTS (SELECT 1 FROM users WHERE id = @userId)",
            connection
        );

        usersExistCommand.Parameters.AddWithValue("userId", userId);

        var exists = usersExistCommand.ExecuteScalar() != null;

        if (!exists)
            return new BadRequestObjectResult(new { error = $"User with id {userId} does not exist." });

        var addUsersCommand = new NpgsqlCommand(
            """
            insert into chatusers (chatid, userid, createdat) values (@chatId, @userId1, now());
                                insert into chatusers (chatid, userid, createdat) values (@chatId, @userId2, now());
            """,
            connection);

        addUsersCommand.Parameters.AddWithValue("chatId", chatId);
        addUsersCommand.Parameters.AddWithValue("userId1", userId);
        addUsersCommand.Parameters.AddWithValue("userId2", Request.GetCurrentUserId());
        
        addUsersCommand.ExecuteNonQuery();

        return new OkObjectResult("Users added to chat successfully.");
    }

    public AddChatUserRequest Request { get; } = request;
}