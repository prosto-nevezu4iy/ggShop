namespace OrderService.Application.DTOs;

public record OrderSummaryDto
{
    public Guid Id { get; init; }
    public decimal TotalAmount { get; init; }
    public string Status { get; init; }
    public DateTime OrderDate { get; init; }
}