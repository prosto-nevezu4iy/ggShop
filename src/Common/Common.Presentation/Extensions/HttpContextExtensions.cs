using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Common.Presentation.Extensions;

public static class HttpContextExtensions
{
    public static Guid GetUserIdentity(this HttpContext httpContext)
    {
        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return Guid.TryParse(userIdClaim, out var userId)
            ? userId
            : Guid.Empty;
    }
}