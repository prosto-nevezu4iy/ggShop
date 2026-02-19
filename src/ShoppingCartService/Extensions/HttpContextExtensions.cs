using Common.Infrastructure.Authentication;
using static Common.Application.Constants.IdentityConstants;

namespace ShoppingCartService.Extensions;

public static class HttpContextExtensions
{
    public static Guid GetUserId(this HttpContext httpContext)
    {
        var userIdentity = httpContext.User.Identity;

        if (userIdentity is { IsAuthenticated: true })
        {
            return httpContext.User.GetUserIdentity();
        }

        if (httpContext.Request.Cookies.ContainsKey(CookieName))
        {
            return Guid.TryParse(httpContext.Request.Cookies[CookieName], out var parsed)
                ? parsed
                : Guid.Empty;
        }

        var userId = Guid.NewGuid();
        var cookieOptions = new CookieOptions
        {
            IsEssential = true,
            Expires = DateTime.Today.AddMonths(1)
        };

        httpContext.Response.Cookies.Append(CookieName, userId.ToString(), cookieOptions);

        return userId;
    }
}