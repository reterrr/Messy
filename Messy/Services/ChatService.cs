using Messy.Actions.Chat;
using Messy.Actions.ChatUser;
using Messy.Helpers;
using Messy.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Messy.Services;

[ApiController]
[Route("chats/{id:long}")]
[UserInChat]
public class ChatService : ControllerBase
{
    [HttpGet("")]
    public IActionResult Get([FromRoute] long id)
    {
        return ActionResolver<GetChatAction, GetChatRequest>
            .Resolve(new GetChatRequest { ChatId = id });
    }

    [HttpPost("users")]
    [UserHasPermission(PermissionType.AddToChat)]
    public IActionResult AddUsers([FromBody] AddGroupUserRequest request, [FromRoute] long id)
    {
        request.ChatId = id;
        
        return ActionResolver<AddGroupUserAction, AddGroupUserRequest>
            .Resolve(request);
    }

    [HttpDelete("")]
    [UserHasPermission(PermissionType.EditChat)]
    public IActionResult Delete([FromRoute] long id)
    {
        return ActionResolver<DeleteChatAction, DeleteChatRequest>
            .Resolve(new DeleteChatRequest
            {
                ChatId = id
            });
    }

    [HttpPost("users/messages")]
    [UserHasPermission(PermissionType.MessageToChat)]
    public IActionResult CreateMessage([FromForm] CreateMessageRequest request, [FromRoute] long id)
    {
        request.ChatId = id;
        
        return ActionResolver<CreateMessageAction, CreateMessageRequest>
            .Resolve(request);
    }
    
    [HttpPut("users/messages/{messageId:long}")]
    [UserHasPermission(PermissionType.EditChat)]
    public IActionResult UpdateMessage([FromBody] UpdateMessageRequest request, [FromRoute] long id, [FromRoute] long messageId)
    {
        request.ChatId = id;
        request.MessageId = messageId;

        return ActionResolver<UpdateMessageAction, UpdateMessageRequest>
            .Resolve(request);
    }

    [HttpDelete("users/messages/{messageId:long}")]
    [UserHasPermission(PermissionType.RemoveFromChat)]
    public IActionResult DeleteMessage([FromRoute] long id, [FromRoute] long messageId)
    {
        return ActionResolver<DeleteMessageAction, DeleteMessageRequest>
            .Resolve(new DeleteMessageRequest
            {
                ChatId = id,
                MessageId = messageId
            });
    }
}