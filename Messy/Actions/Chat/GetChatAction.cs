using Messy.Helpers;
using Messy.Models;
using Messy.Requests;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Messy.Actions.Chat;

public class GetChatAction(GetChatRequest request) : IAction<GetChatRequest>
{
    public static IAction<GetChatRequest> Make(GetChatRequest request)
    {
        return new GetChatAction(request);
    }

    public IActionResult Execute()
    {
        var chatId = Request.ChatId;

        using var connection = NpgslqConnector.CreateConnection();
        connection.Open();

        
        var getChatCommand = new NpgsqlCommand("""
                                                       SELECT 
                                                           c.id AS chat_id, 
                                                           c.title, 
                                                           c.type, 
                                                           c.createdat, 
                                                           c.updatedat, 
                                                           c.deletedat,
                                                           cu.userid, 
                                                           cu.createdat AS cu_createdat, 
                                                           cu.updatedat AS cu_updatedat, 
                                                           cu.deletedat AS cu_deletedat
                                                       FROM 
                                                           chats c 
                                                       LEFT JOIN 
                                                           chatusers cu ON cu.chatid = c.id
                                                       WHERE 
                                                           c.id = @chatId
                                                   
                                               """, connection);

        getChatCommand.Parameters.AddWithValue("chatId", chatId);

        var reader = getChatCommand.ExecuteReader();

        Models.Chat? chat = null;
        var chatUsers = new List<Models.ChatUser>();

        while (reader.Read())
        {
            if (chat == null)
            {
                
                chat = new Models.Chat
                {
                    Id = reader.GetInt32(reader.GetOrdinal("chat_id")),
                    Title = reader.IsDBNull(reader.GetOrdinal("title"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("title")),
                    Type = (ChatType)reader.GetInt16(reader.GetOrdinal("type")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("createdat")),
                    UpdatedAt = reader.IsDBNull(reader.GetOrdinal("updatedat"))
                        ? null
                        : reader.GetDateTime(reader.GetOrdinal("updatedat")),
                    DeletedAt = reader.IsDBNull(reader.GetOrdinal("deletedat"))
                        ? null
                        : reader.GetDateTime(reader.GetOrdinal("deletedat")),
                };
            }

            
            if (!reader.IsDBNull(reader.GetOrdinal("userid")))
            {
                var chatUser = new Models.ChatUser
                {
                    UserId = reader.GetInt64(reader.GetOrdinal("userid")),
                    ChatId = chatId,
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("cu_createdat")),
                    UpdatedAt = reader.IsDBNull(reader.GetOrdinal("cu_updatedat"))
                        ? null
                        : reader.GetDateTime(reader.GetOrdinal("cu_updatedat")),
                    DeletedAt = reader.IsDBNull(reader.GetOrdinal("cu_deletedat"))
                        ? null
                        : reader.GetDateTime(reader.GetOrdinal("cu_deletedat")),
                    
                    Messages = new List<Message>(),
                    Roles = new List<Role>(),
                    Permissions = new List<Permission>()
                };
                chatUsers.Add(chatUser);
            }
        }

        reader.Close();

        
        if (chat != null)
        {
            chat.ChatUsers = chatUsers;
        }

        return new OkObjectResult(chat);
    }


    public GetChatRequest Request { get; } = request;
}