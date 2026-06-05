using Common.Infrastructure.Authentication;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using ShoppingCartService.Abstractions;
using ShoppingCartService.Extensions;

namespace ShoppingCartService.Services;

[Authorize]
public class GrpcShoppingCartService(
    IShoppingCartRepository shoppingCartRepository,
    ILogger<GrpcShoppingCartService> logger)
    : GrpcShoppingCart.GrpcShoppingCartBase
{
    public override async Task<GetShoppingCartResponse> GetShoppingCart(GetShoppingCartRequest request, ServerCallContext context)
    {
        var userId = context.GetHttpContext().User.GetUserIdentity();

        logger.LogDebug("Begin GetShoppingCart call from method {Method} for basket id {Id}", context.Method, userId);

        var data = await shoppingCartRepository.GetShoppingCartAsync(userId);

        return data is not null ? data.ToGetShoppingCartResponse() : new();
    }
}
