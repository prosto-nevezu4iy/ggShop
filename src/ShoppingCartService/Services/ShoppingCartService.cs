using Contracts.Constants;
using ShoppingCartService.DTOs;
using ShoppingCartService.Entities;
using ShoppingCartService.Extensions;
using ShoppingCartService.Repositories;

namespace ShoppingCartService.Services;

public class ShoppingCartService : IShoppingCartService
{
    private readonly IShoppingCartRepository _shoppingCartRepository;
    private readonly ILogger<ShoppingCartService> _logger;

    public ShoppingCartService(IShoppingCartRepository shoppingCartRepository, ILogger<ShoppingCartService> logger)
    {
        _shoppingCartRepository = shoppingCartRepository;
        _logger = logger;
    }

    public async Task<ShoppingCartResponse> GetShoppingCartAsync(HttpContext httpContext)
    {
        // Get user from identity
        var userId = GetUserId(httpContext);
        if (string.IsNullOrEmpty(userId))
        {
            return new();
        }

        _logger.LogDebug("Begin GetShoppingCart call from method {Method} for basket id {Id}", nameof(GetShoppingCartAsync), userId);

        var data = await _shoppingCartRepository.GetBasketAsync(userId);

        if (data is not null)
        {
            return data.ToShoppingCartResponse();
        }

        return new();
    }

    public async Task<ShoppingCartResponse> AddItemToCartAsync(AddCartItemRequest request, HttpContext httpContext)
    {
        var userId = GetUserId(httpContext);
        if (string.IsNullOrEmpty(userId))
        {
            throw new UnauthorizedAccessException("User not authenticated");
        }

        var cart = await _shoppingCartRepository.GetBasketAsync(userId) ?? new UserShoppingCart { UserId = userId };

        var existingItem = cart.Items.FirstOrDefault(i => i.GameId == request.GameId);
        if (existingItem != null)
        {
            existingItem.Quantity += request.Quantity; // if exists, increase quantity
        }
        else
        {
            cart.Items.Add(new Entities.ShoppingCartItem
            {
                GameId = request.GameId,
                Name = request.Name,
                Price = request.Price,
                Quantity = request.Quantity,
                ImageUrl = request.ImageUrl
            });
        }

        var response = await _shoppingCartRepository.UpdateBasketAsync(cart);

        return response.ToShoppingCartResponse();
    }

    public async Task<ShoppingCartResponse> UpdateItemQuantityAsync(Guid gameId, HttpContext httpContext)
    {
        var userId = GetUserId(httpContext);
        if (string.IsNullOrEmpty(userId))
        {
            throw new UnauthorizedAccessException("User not authenticated");
        }

        var cart = await _shoppingCartRepository.GetBasketAsync(userId);
        if (cart == null)
        {
            throw new KeyNotFoundException("Shopping cart not found");
        }

        var item = cart.Items.FirstOrDefault(i => i.GameId == gameId);
        if (item == null)
        {
            throw new KeyNotFoundException("Item not found in cart");
        }

        item.Quantity += 1;

        var updatedCart = await _shoppingCartRepository.UpdateBasketAsync(cart);

        return updatedCart.ToShoppingCartResponse();
    }

    public async Task DeleteShoppingCartAsync(HttpContext httpContext)
    {
        var userId = GetUserId(httpContext);
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogError("User does not exist");
            throw new UnauthorizedAccessException("The caller is not authenticated.");
        }

        await _shoppingCartRepository.DeleteBasketAsync(userId);
    }

    private static string GetUserId(HttpContext context)
    {
        var userIdentity = context.User.Identity;

        if (userIdentity is { IsAuthenticated: true })
        {
            return context.GetUserIdentity();
        }

        if (context.Request.Cookies.ContainsKey(BasketConstants.CookieName))
        {
            return context.Request.Cookies[BasketConstants.CookieName];
        }

        var userId = Guid.NewGuid().ToString();
        var cookieOptions = new CookieOptions
        {
            IsEssential = true,
            Expires = DateTime.Today.AddMonths(1)
        };

        context.Response.Cookies.Append(BasketConstants.CookieName, userId, cookieOptions);

        return userId;
    }
}