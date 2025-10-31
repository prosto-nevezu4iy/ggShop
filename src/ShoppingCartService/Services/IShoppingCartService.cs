using ShoppingCartService.DTOs;
using ShoppingCartService.Entities;

namespace ShoppingCartService.Services;

public interface IShoppingCartService
{
    Task<ShoppingCartResponse> GetShoppingCartAsync(HttpContext httpContext);
    Task<ShoppingCartResponse> AddItemToCartAsync(AddCartItemRequest request, HttpContext httpContext);
    Task<ShoppingCartResponse> UpdateItemQuantityAsync(Guid gameId, HttpContext httpContext);
    Task DeleteShoppingCartAsync(HttpContext httpContext);
}