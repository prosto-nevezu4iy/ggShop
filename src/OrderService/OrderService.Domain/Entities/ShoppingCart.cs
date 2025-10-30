namespace OrderService.Domain.Entities;

public class ShoppingCart
{
    public List<ShoppingCartItem> Items { get; set; } = new();
}