using Messy.Actions;
using Messy.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Messy;

public abstract class ActionResolver<TAction, TRequest>
    where TAction : IAction<TRequest>
    where TRequest : Request
{
    public static IActionResult Resolve(TRequest request)
    {
        return TAction
            .Make(request)
            .Execute();
    }
}