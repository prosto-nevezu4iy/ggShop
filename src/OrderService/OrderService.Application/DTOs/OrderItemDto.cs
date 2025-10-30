namespace OrderService.Application.DTOs;

public record OrderItemDto
{
    public Guid GameId { get; init; }
    public string Name { get; init; }
    public decimal Price { get; init; }
    public int Quantity { get; init; }
    public string ImageUrl { get; init; }
}