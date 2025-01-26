using Messy.Helpers;
using Messy.Requests;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Messy.Actions.Chat;

public class DeleteChatAction(DeleteChatRequest request) : IAction<DeleteChatRequest>
{
    public static IAction<DeleteChatRequest> Make(DeleteChatRequest request)
    {
        return new DeleteChatAction(request);
    }

    public IActionResult Execute()
    {
        using var connection = NpgslqConnector.CreateConnection();

        connection.Open();

        var deleteChatCommand = new NpgsqlCommand(
            "delete from chats where id = @chat_id",
            connection
        );
        
        deleteChatCommand.Parameters.AddWithValue("chat_id", Request.ChatId);

        deleteChatCommand.ExecuteNonQuery();
        
        return new OkObjectResult("Success");
    }

    public DeleteChatRequest Request { get; } = request;
}