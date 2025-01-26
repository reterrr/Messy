using Messy.Helpers;
using Messy.Requests;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Messy.Actions.ChatUser;

public class AddGroupUserAction(AddGroupUserRequest request) : IAction<AddGroupUserRequest>
{
    public static IAction<AddGroupUserRequest> Make(AddGroupUserRequest request)
    {
        return new AddGroupUserAction(request);
    }

    public IActionResult Execute()
    {
        var chatId = Request.ChatId;
        var userIds = Request.UserIds;

        using var connection = NpgslqConnector.CreateConnection();
        connection.Open();

        var chatExistsCommand = new NpgsqlCommand(
            "SELECT 1 FROM chats WHERE id = @chatId and type = @type",
            connection
        );
        chatExistsCommand.Parameters.AddWithValue("chatId", chatId);
        chatExistsCommand.Parameters.AddWithValue("type", (short)ChatType.ManyToMany);

        var chatExists = chatExistsCommand.ExecuteScalar() != null;
        if (!chatExists)
        {
            return new BadRequestObjectResult(new { error = $"Chat with id {chatId} does not exist." });
        }

        var userIdsParameter = string.Join(",", userIds);
        var usersExistCommand = new NpgsqlCommand(
            $"SELECT id FROM users WHERE id = ANY(ARRAY[{userIdsParameter}]::bigint[])",
            connection
        );

        using var reader = usersExistCommand.ExecuteReader();
        var existingUserIds = new HashSet<long>();
        while (reader.Read())
        {
            existingUserIds.Add(reader.GetInt64(0));
        }

        reader.Close();

        var invalidUserIds = userIds.Except(existingUserIds).ToArray();
        if (invalidUserIds.Any())
        {
            return new BadRequestObjectResult(new
            {
                error = "Some user IDs do not exist.",
                invalidUserIds
            });
        }

        var usersInChat =
            new NpgsqlCommand(
                $"select userid from chatusers where chatid = @chatId and userid = ANY(ARRAY[{userIdsParameter}]::bigint[])",
                connection
            );

        using var reader2 = usersInChat.ExecuteReader();
        var existsUserIds2 = new HashSet<long>();
        while (reader2.Read())
        {
            existsUserIds2.Add(reader2.GetInt32(0));
        }

        reader2.Close();
        
        if (existsUserIds2.Any())
        {
            return new BadRequestObjectResult(
                new
                {
                    error = "Some user IDs exist.",
                    existsUserIds2
                }
            );
        }
        
        var insertChatUsersCommand = new NpgsqlCommand(
            "CALL add_users_to_chat(@users, @chatId)",
            connection
        );

        insertChatUsersCommand.Parameters.AddWithValue("users", userIds);
        insertChatUsersCommand.Parameters.AddWithValue("chatId", chatId);

        insertChatUsersCommand.ExecuteNonQuery();


        userIds.Remove(Request.GetCurrentUserId());

        var makeUsersCommand = new NpgsqlCommand(
            "call assign_certain_role(@users, @chatId, @roleType)",
            connection
        );

        makeUsersCommand.Parameters.AddWithValue("users", userIds);
        makeUsersCommand.Parameters.AddWithValue("chatId", chatId);
        makeUsersCommand.Parameters.AddWithValue("roleType", (short)RoleType.User);

        makeUsersCommand.ExecuteNonQuery();

        return new OkObjectResult("Users added to chat successfully.");
    }

    public AddGroupUserRequest Request { get; } = request;
}