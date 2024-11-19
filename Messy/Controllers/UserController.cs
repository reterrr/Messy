using Microsoft.AspNetCore.Mvc;

namespace Messy.Controllers;


[Route("[controller]/[action]")]
[ApiController]
public class UserController : ControllerBase
{
    [ActionName("2")]
    public int E()
    {
        return 2;
    }
    
    [Route("1")]
    public int Dd()
    {
        return 1;
    }
    
    
}