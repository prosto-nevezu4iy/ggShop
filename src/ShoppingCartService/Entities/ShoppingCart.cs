using Common.Domain;
using ShoppingCartService.Configurations;
using ShoppingCartService.Errors;

namespace ShoppingCartService.Entities;

public class ShoppingCart
{
    public Guid UserId { get; }

    private readonly List<ShoppingCartItem> _items = new();

    public IReadOnlyCollection<ShoppingCartItem> Items => _items.AsReadOnly();

    public ShoppingCart(Guid userId)
    {
        UserId = userId;
    }

    public void AddItem(Guid gameId, string name, decimal price, byte quantity, string imageUrl)
    {
        _items.Add(new ShoppingCartItem(gameId, name, price, quantity, imageUrl));
    }

    public Result AddItem(Guid gameId, string name, decimal price, string imageUrl, ShoppingCartSettings shoppingCartSettings)
    {
        var existingItem = _items.FirstOrDefault(i => i.GameId == gameId);

        if (existingItem is not null)
        {
            return UpdateItemQuantity(gameId, 1, shoppingCartSettings);
        }

        AddItem(gameId, name, price, 1, imageUrl);

        return Result.Success();
    }

    public Result UpdateItemQuantity(Guid gameId, int delta, ShoppingCartSettings shoppingCartSettings)
    {
        var item = _items.FirstOrDefault(i => i.GameId == gameId);

        if (item is null)
        {
            return ShoppingCartErrors.ItemNotFound(gameId);
        }

        var newQuantity = item.Quantity + delta;

        if (newQuantity <= 0)
        {
            return RemoveItem(item);
        }

        if (newQuantity > shoppingCartSettings.MaxItemsPerCart)
        {
            return ShoppingCartErrors.InvalidQuantity(item.GameId, newQuantity);
        }

        item.UpdateQuantity((byte)delta);

        return Result.Success();
    }

    private Result RemoveItem(ShoppingCartItem item)
    {
        _items.Remove(item);

        return Result.Success();
    }

    public int RemoveItemsByGameId(Guid gameId)
    {
        return _items.RemoveAll(i => i.GameId == gameId);
    }
}