using Messy.Contexts;
using Messy.Helpers;
using Microsoft.AspNetCore.Mvc;
using LoginRequest = Messy.Requests.LoginRequest;

namespace Messy.Actions.Auth;

public class LoginUserAction : IAction<LoginRequest>
{
    private LoginUserAction(LoginRequest request)
    {
        Request = request;
    }

    public IActionResult Execute()
    {
        var user = AuthValidator.Make()
            .ExtractValidUser(Request.Username, Request.Password);
            
        if (user == null)
            return new UnauthorizedObjectResult("Invalid username or password");
            
        var token = JwTGenerator.Generate(user);

        return new OkObjectResult(token);
    }

    public LoginRequest Request { get; }

    public static IAction<LoginRequest> Make(LoginRequest request)
    {
        return new LoginUserAction(request);
    }
}
