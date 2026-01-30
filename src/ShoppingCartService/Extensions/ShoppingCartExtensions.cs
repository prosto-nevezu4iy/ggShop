using ShoppingCartService.DTOs;
using ShoppingCartService.Entities;

namespace ShoppingCartService.Extensions;

public static class ShoppingCartExtensions
{
    public static ShoppingCartDto ToDto(this ShoppingCart shoppingCart)
    {
        return new ShoppingCartDto(shoppingCart.Items.Select(x =>
            new ShoppingCartItemDto(x.GameId, x.Name, x.Price, x.Quantity, x.ImageUrl)).ToList());
    }

    public static GetShoppingCartResponse ToGetShoppingCartResponse(this ShoppingCart shoppingCart)
    {
        var response = new GetShoppingCartResponse();

        foreach (var item in shoppingCart.Items)
        {
            response.Items.Add(new ShoppingCartItem
            {
                GameId = item.GameId.ToString(),
                Quantity = item.Quantity,
            });
        }

        return response;
    }
}