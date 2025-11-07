namespace OrderService.Domain.Entities;

public class GameItem
{
    public Guid GameId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int AvailableStock { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}