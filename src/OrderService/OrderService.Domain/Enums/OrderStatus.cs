namespace OrderService.Domain.Enums;

public enum OrderStatus
{
    Created,
    Processing,
    StockConfirmed,
    Paid,
    Completed,
    Cancelled
}