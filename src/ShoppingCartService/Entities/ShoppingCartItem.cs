namespace ShoppingCartService.Entities;

public class ShoppingCartItem
{
    public Guid GameId { get; set; }
    public int Quantity { get; set; }
}