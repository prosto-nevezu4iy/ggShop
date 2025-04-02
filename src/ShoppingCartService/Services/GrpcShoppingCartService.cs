using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using ShoppingCartService.Entities;
using ShoppingCartService.Repositories;

namespace ShoppingCartService.Services;

public class GrpcShoppingCartService : ShoppingCart.ShoppingCartBase
{
    private readonly IShoppingCartRepository _shoppingCartRepository;
    private readonly ILogger<GrpcShoppingCartService> _logger;

    public GrpcShoppingCartService(IShoppingCartRepository shoppingCartRepository, ILogger<GrpcShoppingCartService> logger)
    {
        _shoppingCartRepository = shoppingCartRepository;
        _logger = logger;
    }

    [AllowAnonymous]
    public override async Task<UserShoppingCartResponse> GetShoppingCart(GetShoppingCartRequest request, ServerCallContext context)
    {
        // Get user from identity
        var userId = "ce280121-e2bd-49ef-91c1-ec74df096115";
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

    [AllowAnonymous]
    public override async Task<UserShoppingCartResponse> UpdateShoppingCart(UpdateShoppingCartRequest request, ServerCallContext context)
    {
        // Get user from identity
        var userId = "ce280121-e2bd-49ef-91c1-ec74df096115";
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogError("User does not exist");
            //ThrowNotAuthenticated();
        }

        _logger.LogInformation("Begin UpdateShoppingCart call from method {Method} for basket id {Id}", context.Method, userId);

        var userShoppingCart = MapToUserShoppingCart(userId, request);
        var response = await _shoppingCartRepository.UpdateBasketAsync(userShoppingCart);
        if (response is null)
        {

            _logger.LogError("ShoppingCart does not exist");

            //ThrowBasketDoesNotExist(userId);
        }

        return MapToUserShoppingCartResponse(response);
    }

    public override async Task<DeleteUserShoppingCartResponse> DeleteShoppingCart(DeleteShoppingCartRequest request, ServerCallContext context)
    {
        // Get user from identity
        var userId = "ce280121-e2bd-49ef-91c1-ec74df096115";
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogError("User does not exist");
            //ThrowNotAuthenticated();
        }

        await _shoppingCartRepository.DeleteBasketAsync(userId);

        return new();
    }

    private static UserShoppingCartResponse MapToUserShoppingCartResponse(UserShoppingCart userShoppingCart)
    {
        var response = new UserShoppingCartResponse();

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

    private static UserShoppingCart MapToUserShoppingCart(string userId, UpdateShoppingCartRequest request)
    {
        var response = new UserShoppingCart
        {
            UserId = userId
        };

        foreach (var item in request.Items)
        {
            response.Items.Add(new()
            {
                GameId = Guid.Parse(item.GameId),
                Quantity = item.Quantity,
            });
        }

        return response;
    }
}
