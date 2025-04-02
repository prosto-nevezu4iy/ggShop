namespace ShoppingCartService.Repositories;
using ShoppingCartService.Entities;

public interface IShoppingCartRepository
{
    Task<UserShoppingCart> GetBasketAsync(string userId);
    Task<UserShoppingCart> UpdateBasketAsync(UserShoppingCart shoppingCart);
    Task<bool> DeleteBasketAsync(string id);
}