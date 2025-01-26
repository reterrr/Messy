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
        return request switch
        {
            CreateOneToOneRequest createChatRequest =>
                ActionResolver<CreateOneToOneAction, CreateOneToOneRequest>
                    .Resolve(createChatRequest),

            CreateManyToManyRequest createGroupRequest => 
                ActionResolver<CreateManyToManyAction, CreateManyToManyRequest>
                    .Resolve(createGroupRequest),

            _ => BadRequest()
        };
    }
}