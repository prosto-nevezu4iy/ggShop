using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using ShoppingCartService.Entities;
using ShoppingCartService.Extensions;
using ShoppingCartService.Repositories;

namespace ShoppingCartService.Services;

[Authorize]
public class GrpcShoppingCartService : GrpcShoppingCart.GrpcShoppingCartBase
{
    private readonly IShoppingCartRepository _shoppingCartRepository;
    private readonly ILogger<GrpcShoppingCartService> _logger;

    public GrpcShoppingCartService(IShoppingCartRepository shoppingCartRepository, ILogger<GrpcShoppingCartService> logger)
    {
        _shoppingCartRepository = shoppingCartRepository;
        _logger = logger;
    }

    public override async Task<GetShoppingCartResponse> GetShoppingCart(GetShoppingCartRequest request, ServerCallContext context)
    {
        var userId = context.GetUserIdentity();
        if (string.IsNullOrEmpty(userId))
        {
            return new();
        }

        _logger.LogDebug("Begin GetShoppingCart call from method {Method} for basket id {Id}", context.Method, userId);

        var data = await _shoppingCartRepository.GetBasketAsync(userId);

        if (data is not null)
        {
            return MapToUserShoppingCartResponse(data);
        }

        return new();
    }

    private static GetShoppingCartResponse MapToUserShoppingCartResponse(UserShoppingCart userShoppingCart)
    {
        var response = new GetShoppingCartResponse();

        foreach (var item in userShoppingCart.Items)
        {
            response.Items.Add(new ShoppingCartItem()
            {
                GameId = item.GameId.ToString(),
                Quantity = item.Quantity,
            });
        }

        return response;
    }
}
