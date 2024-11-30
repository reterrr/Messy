using Messy.Actions.User;
using Messy.Helpers;
using Messy.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Messy.Actions.Auth;

public class RegisterUserAction(RegisterRequest request) : IAction<RegisterRequest>
{
    public static IAction<RegisterRequest> Make(RegisterRequest request)
    {
        return new RegisterUserAction(request);
    }

    public IActionResult Execute()
    {
        if (AuthValidator.Make().UserExists(Request.UserName))
            return new BadRequestObjectResult("Such user already exists.");

        ActionResolver<CreateUserAction, RegisterRequest>
            .Resolve(Request);
        
        return new OkObjectResult("Registered Successfully");
    }

    private bool UserExists(string matching)
    {
        
        return false;
    }
    
    public RegisterRequest Request { get; } = request;
}