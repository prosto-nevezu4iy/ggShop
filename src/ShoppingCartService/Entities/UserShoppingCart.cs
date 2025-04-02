namespace ShoppingCartService.Entities;

public class UserShoppingCart
{
    public string UserId { get; set; }
    public List<ShoppingCartItem> Items { get; set; } = new();
}
