using Messy.Helpers;
using Messy.Requests;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Messy.Actions.User;

public class CreateUserAction : IAction<RegisterRequest>
{
    private CreateUserAction(RegisterRequest request)
    {
        Request = request;
    }
    public static IAction<RegisterRequest> Make(RegisterRequest request)
    {
        return new CreateUserAction(request);
    }

    public IActionResult Execute()
    {
        var passwordHasher = new PasswordHasher<Models.User>();
        var hashedPassword = passwordHasher.HashPassword(null, Request.Password);

        // Create new user
        var user = new Models.User
        {
            Name = Request.Name,
            UserName = Request.UserName,
            Password = hashedPassword,  // Store the hashed password
            CreatedAt = DateTime.UtcNow
        };

        // Save the user to the database
        using (var context = MessyDbContextFactory.CreateDbContext())
        {
            context.Set<Models.User>().Add(user);
            context.SaveChanges();
        }

        // Return a success response
        return new OkObjectResult(new { Message = "User created successfully." });
    }

    public RegisterRequest Request { get; }
}