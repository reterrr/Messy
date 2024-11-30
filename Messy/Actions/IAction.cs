using Messy.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Messy.Actions;

public interface IAction<TRequest>
    where TRequest : Request
{
    public static abstract IAction<TRequest> Make(TRequest request);

    public IActionResult Execute();
    
    public TRequest Request { get; }
}