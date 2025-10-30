namespace OrderService.Application.DTOs;

public record OrderDto
{
    public Guid Id { get; init; }
    public decimal TotalAmount { get; init; }
    public string Status { get; init; }
    public DateTime OrderDate { get; init; }
    public List<OrderItemDto> OrderItems { get; init; }
}