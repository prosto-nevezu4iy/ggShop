namespace ShoppingCartService.Entities;

public class ShoppingCartItem
{
    public Guid GameId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string ImageUrl { get; set; }
}