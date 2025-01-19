using System.Net.WebSockets;
using System.Text;
using Messy.Actions.User;
using Messy.Requests;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Messy.Controllers;

[ApiController]
[Route("users")]
public class UserController : ControllerBase
{
    [HttpDelete("")]
    public IActionResult DeleteUser([FromBody] DeleteUserRequest request)
    {
        return ActionResolver<DeleteUserAction, DeleteUserRequest>
            .Resolve(request);
    }

    [HttpGet("search")]
    public async Task SearchUserWebSocket()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            await HandleWebSocketSearch(webSocket);
        }
        else
        {
            HttpContext.Response.StatusCode = 400;
        }
    }

    private async Task HandleWebSocketSearch(WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];
        var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

        while (!result.CloseStatus.HasValue)
        {
            var requestJson = Encoding.UTF8.GetString(buffer, 0, result.Count);
            var request = JsonConvert.DeserializeObject<SearchUserRequest>(requestJson);

            var response = ActionResolver<SearchUserAction, SearchUserRequest>
                .Resolve(request);

            var responseJson = JsonConvert.SerializeObject(response);
            var responseBuffer = Encoding.UTF8.GetBytes(responseJson);

            await webSocket.SendAsync(new ArraySegment<byte>(responseBuffer), WebSocketMessageType.Text, true, CancellationToken.None);

            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        }

        await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
    }
}