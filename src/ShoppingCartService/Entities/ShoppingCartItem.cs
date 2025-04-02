namespace ShoppingCartService.Entities;

public class ShoppingCartItem
{
    public Guid Id { get; set; }
    public Guid GameId { get; set; }
    public string GameName { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string ImageUrl { get; set; }
}