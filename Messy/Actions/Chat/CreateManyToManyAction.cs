using Messy.Actions.ChatUser;
using Messy.Helpers;
using Messy.Requests;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Messy.Actions.Chat;

public class CreateManyToManyAction(CreateManyToManyRequest request) : IAction<CreateManyToManyRequest>
{
    public static IAction<CreateManyToManyRequest> Make(CreateManyToManyRequest request)
    {
        return new CreateManyToManyAction(request);
    }

    public IActionResult Execute()
    {
        var userIds = Request.UserIds;
        
        using var connection = NpgslqConnector.CreateConnection();
        connection.Open();

        var createGroupCommand = new NpgsqlCommand(
            "insert into chats(type, createdat) values (@type, now()) returning id",
            connection);
        
        createGroupCommand.Parameters.AddWithValue("type", (short)ChatType.ManyToMany);
        
        var chatId = (long)createGroupCommand.ExecuteScalar();

        var addUsersRequest = new AddGroupUserRequest
        {
            ChatId = chatId,
            UserIds = userIds
        };
        
        var makeOwnerCommand = new NpgsqlCommand(
            """
            insert into chatusers(chatid, userid, createdat) values (@chatId, @userId, now());
            insert into chatuserroles (chatuserid, roleid) values (
                (select id from chatusers where chatid = @chatId and userid = @userId),                                                       
            (select id from roles where type = @roleType))
            """,
            connection
        );
        makeOwnerCommand.Parameters.AddWithValue("chatId", chatId);
        makeOwnerCommand.Parameters.AddWithValue("userId", Request.GetCurrentUserId());
        makeOwnerCommand.Parameters.AddWithValue("roleType", (short)RoleType.Owner);
        
        makeOwnerCommand.ExecuteNonQuery();
        
        return ActionResolver<AddGroupUserAction, AddGroupUserRequest>
            .Resolve(addUsersRequest);
    }

    public CreateManyToManyRequest Request { get; } = request;
}