using System.Security.Claims;
using Contracts.Constants;

namespace OrderService.Api.Extensions;

public static class ContextIdentityExtensions
{
    public static Guid GetUserIdentity(this HttpContext context)
    {
        var userId = context?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return Guid.TryParse(userId, out var id) ? id : Guid.Empty;
    }

    public static string GetToken(this HttpContext context)
    {
        return context.Request.Headers["Authorization"]
            .ToString()
            .Replace("Bearer ", "");
    }
}