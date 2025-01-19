using System.Security.Claims;
using Microsoft.EntityFrameworkCore.Design;

namespace Messy.Requests;

public class Request
{
   private static IHttpContextAccessor _httpContextAccessor;
   public static void Init(IHttpContextAccessor httpContextAccessor)
   {
      _httpContextAccessor = httpContextAccessor;
   }

   public long GetCurrentUserId()
   {
      return long.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
   }
}