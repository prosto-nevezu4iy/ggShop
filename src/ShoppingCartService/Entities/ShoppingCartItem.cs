namespace ShoppingCartService.Entities;

public class ShoppingCartItem
{
    public Guid GameId { get; private set; }
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public byte Quantity { get; private set; }
    public string ImageUrl { get; private set; }

    public ShoppingCartItem(Guid gameId, string name, decimal price, byte quantity, string imageUrl)
    {
        GameId = gameId;
        Name = name;
        Price = price;
        Quantity = quantity;
        ImageUrl = imageUrl;
    }

    public void UpdateQuantity(byte amount)
    {
        Quantity += amount;
    }
}