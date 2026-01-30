using ShoppingCartService.Entities;

namespace ShoppingCartService.Abstractions;

public interface IShoppingCartRepository
{
    Task<ShoppingCart> GetShoppingCartAsync(Guid userId);
    Task<ShoppingCart> UpdateShoppingCartAsync(ShoppingCart shoppingCart);
    Task<bool> DeleteShoppingCartAsync(Guid userId);
    Task DeleteShoppingCartsByGameIdAsync(Guid gameId);
    Task TransferAnonymousCartAsync(Guid anonymousId, Guid userId);
}