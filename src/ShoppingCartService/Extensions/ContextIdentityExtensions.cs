using System.Security.Claims;

namespace ShoppingCartService.Extensions;

public static class ContextIdentityExtensions
{
    public static string GetUserIdentity(this HttpContext context)
    {
        return context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}