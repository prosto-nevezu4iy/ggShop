using System.Security.Claims;

namespace Common.Infrastructure.Authentication;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserIdentity(this ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return Guid.TryParse(userIdClaim, out var userId)
            ? userId
            : Guid.Empty;
    }
}