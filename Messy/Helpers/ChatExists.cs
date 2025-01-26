using Microsoft.AspNetCore.Mvc.Filters;

namespace Messy.Helpers;

[AttributeUsage(AttributeTargets.Class)]
public class ChatExists : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        
    }
}