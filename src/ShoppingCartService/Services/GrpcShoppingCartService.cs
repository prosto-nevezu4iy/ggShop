using Contracts.Constants;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using ShoppingCartService.Entities;
using ShoppingCartService.Extensions;
using ShoppingCartService.Repositories;

namespace ShoppingCartService.Services;

public class GrpcShoppingCartService : GrpcShoppingCart.GrpcShoppingCartBase
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
        var userId = GetUserId(context);
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
        var userId = GetUserId(context);
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogError("User does not exist");
            ThrowNotAuthenticated();
        }

        _logger.LogInformation("Begin UpdateShoppingCart call from method {Method} for basket id {Id}", context.Method, userId);

        var userShoppingCart = MapToUserShoppingCart(userId, request);
        var response = await _shoppingCartRepository.UpdateBasketAsync(userShoppingCart);
        if (response is null)
        {
            _logger.LogError("ShoppingCart does not exist");
            ThrowBasketDoesNotExist(userId);
        }

        return MapToUserShoppingCartResponse(response);
    }

    public override async Task<DeleteUserShoppingCartResponse> DeleteShoppingCart(DeleteShoppingCartRequest request, ServerCallContext context)
    {
        // Get user from identity
        var userId = GetUserId(context);
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogError("User does not exist");
            ThrowNotAuthenticated();
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
                Name = item.Name,
                Price = (double) item.Price,
                Quantity = item.Quantity,
                ImageUrl = item.ImageUrl
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
                Name = item.Name,
                Price = (decimal) item.Price,
                Quantity = item.Quantity,
                ImageUrl = item.ImageUrl
            });
        }

        return response;
    }

    private static void ThrowNotAuthenticated() => throw new RpcException(new Status(StatusCode.Unauthenticated, "The caller is not authenticated."));

    private static void ThrowBasketDoesNotExist(string userId) => throw new RpcException(new Status(StatusCode.NotFound, $"ShoppingCart with user id {userId} does not exist"));

    private static string GetUserId(ServerCallContext context)
    {
        var httpContext = context.GetHttpContext();

        var userIdentity = httpContext.User.Identity;

        if (userIdentity is { IsAuthenticated: true })
        {
            return context.GetUserIdentity();
        }

        if (httpContext.Request.Cookies.ContainsKey(BasketConstants.CookieName))
        {
            return httpContext.Request.Cookies[BasketConstants.CookieName];
        }

        var userId = Guid.NewGuid().ToString();
        var cookieOptions = new CookieOptions
        {
            IsEssential = true,
            Expires = DateTime.Today.AddMonths(1)
        };

        httpContext.Response.Cookies.Append(BasketConstants.CookieName, userId, cookieOptions);

        return userId;
    }
}
