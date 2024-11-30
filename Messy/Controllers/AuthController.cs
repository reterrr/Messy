using Messy.Actions.Auth;
using Messy.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoginRequest = Messy.Requests.LoginRequest;

namespace Messy.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        return ActionResolver<LoginUserAction, LoginRequest>
            .Resolve(request);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest request)
    {
        return ActionResolver<RegisterUserAction, RegisterRequest>
            .Resolve(request);
    }
}