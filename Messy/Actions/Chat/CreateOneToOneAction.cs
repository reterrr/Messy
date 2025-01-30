using Messy.Actions.ChatUser;
using Messy.Helpers;
using Messy.Requests;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Messy.Actions.Chat;

public class CreateOneToOneAction(CreateOneToOneRequest request) : IAction<CreateOneToOneRequest>
{
    public static IAction<CreateOneToOneRequest> Make(CreateOneToOneRequest request)
    {
        return new CreateOneToOneAction(request);
    }

    public IActionResult Execute()
    {
        using var connection = NpgsqlConnector.CreateConnection();
        connection.Open();

        if (UsersAreAlreadyInOneToOneChat(connection))
            return new BadRequestObjectResult("Such chat exists");
        
        if (Request.UserId == Request.GetCurrentUserId())
            return new BadRequestObjectResult("You cannot create one to one chat with yourself try group");

        var createChatCommand =
            new NpgsqlCommand(
                "insert into chats(type, createdat) values (@chatType, now()) returning id",
                connection
            );

        createChatCommand.Parameters.AddWithValue("chatType", (short)ChatType.OneToOne);

        var chatId = (long)createChatCommand.ExecuteScalar();

        var addUsersRequest = new AddChatUserRequest
        {
            userId = Request.UserId,
            chatId = chatId
        };
        
        return ActionResolver<AddChatUserAction, AddChatUserRequest>
            .Resolve(addUsersRequest);
    }

    public CreateOneToOneRequest Request { get; } = request;

    private bool UsersAreAlreadyInOneToOneChat(NpgsqlConnection conn)
    {
        var areInChatCommand = new NpgsqlCommand(
            """
            select exists(select 1
            from chats c
                     join chatusers cu1 on cu1.chatid = c.id
                     join chatusers cu2 on cu2.chatid = c.id
            where c.type = 0
              and (
                cu1.userid = @userId1
                    and cu2.userid = @userId2
                ))
            """
            , conn);

        areInChatCommand.Parameters.AddWithValue("userId1", Request.UserId);
        areInChatCommand.Parameters.AddWithValue("userId2", Request.GetCurrentUserId());

        return (bool)areInChatCommand.ExecuteScalar();
    }
}