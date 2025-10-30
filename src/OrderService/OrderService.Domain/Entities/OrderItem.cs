namespace OrderService.Domain.Entities;

public class OrderItem
{
    public Guid Id { get; private set; }
    public Guid GameId { get; private set; }
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public int Quantity { get; private set; }
    public string ImageUrl { get; private set; }

    public OrderItem(Guid gameId, string name, decimal price, int quantity, string imageUrl)
    {
        GameId = gameId;
        Name = name;
        Price = price;
        Quantity = quantity;
        ImageUrl = imageUrl;
    }
}