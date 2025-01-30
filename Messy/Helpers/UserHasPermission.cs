using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Npgsql;

namespace Messy.Helpers;

[AttributeUsage(AttributeTargets.Method)]
public class UserHasPermission(PermissionType requiredPermissionType) : ActionFilterAttribute
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

        if (long.TryParse(context.RouteData.Values["messageId"] as string, out var messageId))
        {
            var connection = NpgsqlConnector.CreateConnection();
            connection.Open();

            var command = new NpgsqlCommand(
                @"select exists(select 1 
from messages where chatuserid = (select id from chatusers where chatid = @chatId and userid = @userId))",
                connection
            );
            command.Parameters.AddWithValue("chatId", chatId);
            command.Parameters.AddWithValue("userId", userId);

            if ((bool)command.ExecuteScalar()) return;
            
            context.Result = new UnauthorizedResult();
            return;
        }

        try
        {
            using (var connection = NpgsqlConnector.CreateConnection())
            {
                connection.Open();


                using (var chatTypeCommand = new NpgsqlCommand(
                           @"SELECT Type 
                         FROM Chats 
                         WHERE Id = @chatId",
                           connection))
                {
                    chatTypeCommand.Parameters.AddWithValue("chatId", chatId);

                    var chatTypeResult = chatTypeCommand.ExecuteScalar();
                    if (chatTypeResult == null)
                    {
                        context.Result = new NotFoundObjectResult("Chat not found.");
                        return;
                    }

                    if ((int)chatTypeResult == 0)
                    {
                        return;
                    }
                }


                long permissionId;
                using (var permissionCommand = new NpgsqlCommand(
                           @"SELECT Id 
                         FROM Permissions 
                         WHERE Type = @type",
                           connection))
                {
                    permissionCommand.Parameters.AddWithValue("type", (short)requiredPermissionType);

                    var result = permissionCommand.ExecuteScalar();
                    if (result == null)
                    {
                        context.Result = new NotFoundObjectResult("Permission not found.");
                        return;
                    }

                    permissionId = (long)result;
                }


                long chatUserId;
                using (var chatUserCommand = new NpgsqlCommand(
                           @"SELECT Id 
                         FROM ChatUsers 
                         WHERE UserId = @userId AND ChatId = @chatId",
                           connection))
                {
                    chatUserCommand.Parameters.AddWithValue("userId", userId);
                    chatUserCommand.Parameters.AddWithValue("chatId", chatId);

                    var chatUserIdResult = chatUserCommand.ExecuteScalar();
                    if (chatUserIdResult == null)
                    {
                        context.Result = new NotFoundObjectResult("User is not part of the chat.");
                        return;
                    }

                    chatUserId = (long)chatUserIdResult;
                }


                using (var directPermissionCommand = new NpgsqlCommand(
                           @"SELECT EXISTS(
                           SELECT 1 
                           FROM ChatUserPermissions 
                           WHERE ChatUserId = @chatUserId AND PermissionId = @permissionId)",
                           connection))
                {
                    directPermissionCommand.Parameters.AddWithValue("chatUserId", chatUserId);
                    directPermissionCommand.Parameters.AddWithValue("permissionId", permissionId);

                    var hasDirectPermission = (bool)directPermissionCommand.ExecuteScalar();
                    if (hasDirectPermission)
                        return;
                }


                using (var rolePermissionCommand = new NpgsqlCommand(
                           @"SELECT EXISTS(
                           SELECT 1 
                           FROM ChatUserRoles cur
                           JOIN RolePermissions rp ON cur.RoleId = rp.RoleId
                           WHERE cur.ChatUserId = @chatUserId AND rp.PermissionId = @permissionId)",
                           connection))
                {
                    rolePermissionCommand.Parameters.AddWithValue("chatUserId", chatUserId);
                    rolePermissionCommand.Parameters.AddWithValue("permissionId", permissionId);

                    var hasRolePermission = (bool)rolePermissionCommand.ExecuteScalar();
                    if (hasRolePermission)
                        return;
                }


                context.Result = new ForbidResult();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking user permissions: {ex.Message}");
            context.Result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}