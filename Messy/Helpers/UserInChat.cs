using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Npgsql;

namespace Messy.Helpers;

[AttributeUsage(AttributeTargets.Class)]
public class UserInChat : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!long.TryParse(context.RouteData.Values["id"] as string, out var chatId))
        {
            context.Result = new BadRequestObjectResult("Invalid chat ID.");
            return;
        }

        var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        try
        {
            using (var connection = NpgslqConnector.CreateConnection())
            {
                connection.Open();

                using (var userInChatCommand = new NpgsqlCommand(
                           "select exists(select 1 from chatusers where userid = @userid and chatid = @chatid)",
                           connection))
                {
                    userInChatCommand.Parameters.AddWithValue("chatid", chatId);
                    userInChatCommand.Parameters.AddWithValue("userid", userId);

                    var isUserInChat = (bool)userInChatCommand.ExecuteScalar();


                    if (!isUserInChat)
                    {
                        context.Result = new ForbidResult();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking user in chat: {ex.Message}");
            context.Result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}