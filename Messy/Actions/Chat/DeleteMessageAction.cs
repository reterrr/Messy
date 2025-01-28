using Messy.Helpers;
using Messy.Requests;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Messy.Actions.Chat;

public class DeleteMessageAction(DeleteMessageRequest request) : IAction<DeleteMessageRequest>
{
    public static IAction<DeleteMessageRequest> Make(DeleteMessageRequest request)
    {
        return new DeleteMessageAction(request);
    }

    public IActionResult Execute()
    {
        using var connection = NpgslqConnector.CreateConnection();
        connection.Open();

        var deleteCommand = new NpgsqlCommand(
            """
            delete from messages
            where id = @messageId
            """,
            connection);

        deleteCommand.Parameters.AddWithValue("messageId", Request.MessageId);
        deleteCommand.Parameters.AddWithValue("chatId", Request.ChatId);

        var rowsAffected = deleteCommand.ExecuteNonQuery();

        if (rowsAffected == 0)
        {
            return new BadRequestObjectResult("Message not found or you lack permissions.");
        }

        return new OkResult();
    }

    public DeleteMessageRequest Request { get; } = request;
}