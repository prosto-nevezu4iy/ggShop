namespace OrderService.Domain.Entities;

public class Catalog
{
    public List<GameItem> Items { get; set; } = new();
}