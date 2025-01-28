using Messy.Models;
using Microsoft.AspNetCore.Mvc;

namespace Messy.Helpers;

public static class ExcludeDeletedUsers
{
    public static IActionResult exclude(IActionResult result)
    {
        return result switch
        {
            OkObjectResult { Value: IEnumerable<User> users } => new OkObjectResult(users
                .Where(u => u.DeletedAt == null)
                .ToList()),
            OkObjectResult { Value: User user } => new OkObjectResult(user.DeletedAt != null ? "" : user),
            _ => new OkObjectResult("No users found")
        };
    }
}