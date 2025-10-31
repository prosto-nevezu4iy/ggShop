using ShoppingCartService.Entities;

namespace ShoppingCartService.Repositories;

public interface IShoppingCartRepository
{
    Task<UserShoppingCart> GetBasketAsync(string userId);
    Task<UserShoppingCart> UpdateBasketAsync(UserShoppingCart shoppingCart);
    Task DeleteBasketAsync(string userId);
    Task DeleteBasketsByGameIdAsync(Guid gameId);
}