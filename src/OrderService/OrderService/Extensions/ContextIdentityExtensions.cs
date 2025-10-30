using System.Security.Claims;

namespace OrderService.Extensions;

public static class ContextIdentityExtensions
{
    public static Guid GetUserIdentity(this HttpContext context)
    {
        var userId = context?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return Guid.TryParse(userId, out var id) ? id : Guid.Empty;
    }
}