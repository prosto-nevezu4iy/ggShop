using ShoppingCartService.DTOs;
using ShoppingCartService.Entities;

namespace ShoppingCartService.Extensions;

public static class UserShoppingCartExtensions
{
    public static ShoppingCartResponse ToShoppingCartResponse(this UserShoppingCart shoppingCart)
    {
        return new ShoppingCartResponse(shoppingCart.Items.Select(x =>
            new ShoppingCartItemDto(x.GameId, x.Name, x.Price, x.Quantity, x.ImageUrl))
            .ToList());
    }
}