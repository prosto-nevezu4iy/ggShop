using Common.Presentation.Extensions;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using ShoppingCartService.Abstractions;
using ShoppingCartService.Extensions;

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
        var userId = context.GetHttpContext().GetUserIdentity();

        _logger.LogDebug("Begin GetShoppingCart call from method {Method} for basket id {Id}", context.Method, userId);

        var data = await _shoppingCartRepository.GetShoppingCartAsync(userId);

        return data is not null ? data.ToGetShoppingCartResponse() : new();
    }
}
