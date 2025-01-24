using Messy.Helpers;
using Messy.Requests;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Messy.Actions.Chat;

public class CreateChatAction(CreateChatRequest request) : IAction<CreateChatRequest>
{
    public static IAction<CreateChatRequest> Make(CreateChatRequest request)
    {
        return new CreateChatAction(request);
    }

    public IActionResult Execute()
    {
        var chatType = Request.chatType;

        using var connection = NpgslqConnector.CreateConnection();
        connection.Open();

        var createChatCommand =
            new NpgsqlCommand(
                "insert into chats(type, createdat) values (@chatType, now()) returning id",
                connection
            );

        createChatCommand.Parameters.AddWithValue("chatType", (short)chatType);

        var chatId = (long)createChatCommand.ExecuteScalar();
        
        return new OkObjectResult(chatId);
    }
    
    public CreateChatRequest Request { get; } = request;
}