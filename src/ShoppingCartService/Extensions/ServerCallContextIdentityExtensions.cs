using System.Security.Claims;
using Grpc.Core;

namespace ShoppingCartService.Extensions;

public static class ServerCallContextIdentityExtensions
{
    public static string GetUserIdentity(this ServerCallContext context)
    {
        return context.GetHttpContext().User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}