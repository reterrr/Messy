using Messy.Actions.Chat;
using Messy.Requests;
using Microsoft.AspNetCore.Mvc;


namespace Messy.Controllers;

[ApiController]
[Route("chats")]
public class ChatController : ControllerBase
{
    [HttpPost("")]
    public IActionResult Create([FromBody] CreateChatRequest request)
    {
        return ActionResolver<CreateChatAction, CreateChatRequest>
            .Resolve(request);
    }
}