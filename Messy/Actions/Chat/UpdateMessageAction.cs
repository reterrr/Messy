using Messy.Helpers;
using Messy.Requests;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Messy.Actions.Chat;

public class UpdateMessageAction(UpdateMessageRequest request) : IAction<UpdateMessageRequest>
{
    public static IAction<UpdateMessageRequest> Make(UpdateMessageRequest request)
    {
        return new UpdateMessageAction(request);
    }

    public IActionResult Execute()
    {
        using var connection = NpgsqlConnector.CreateConnection();
        connection.Open();

        var updateCommand = new NpgsqlCommand(
            """
            update messages
            set body = @newBody, updatedat = now()
            where id = @messageId
            """, 
            connection);

        updateCommand.Parameters.AddWithValue("newBody", Request.NewBody);
        updateCommand.Parameters.AddWithValue("messageId", Request.MessageId);
        updateCommand.Parameters.AddWithValue("chatId", Request.ChatId);

        var rowsAffected = updateCommand.ExecuteNonQuery();
        
        if (rowsAffected == 0)
        {
            return new BadRequestObjectResult("Message not found or you lack permissions.");
        }

        return new OkResult();
    }

    public UpdateMessageRequest Request { get; } = request;
}