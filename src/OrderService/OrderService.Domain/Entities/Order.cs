using OrderService.Domain.Enums;

namespace OrderService.Domain.Entities;

public class Order
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime OrderDate { get; private set; }
    public OrderStatus Status { get; private set; }
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

    private readonly List<OrderItem> _orderItems = new();


    public Order(Guid userId)
    {
        UserId = userId;
        OrderDate = DateTime.UtcNow;
        Status = OrderStatus.Created;
    }

    public void AddOrderItem(Guid gameId, string name, decimal price, int quantity, string imageUrl)
    {
        var orderItem = new OrderItem(gameId, name, price, quantity, imageUrl);
        _orderItems.Add(orderItem);
    }

    public decimal GetTotalAmount()
    {
        return _orderItems.Sum(item => item.Price * item.Quantity);
    }

    public void SetProcessingStatus()
    {
        if (Status is OrderStatus.Created)
        {
            Status = OrderStatus.Processing;
        }
    }

    public void SetStockConfirmedStatus()
    {
        if (Status is OrderStatus.Processing)
        {
            Status = OrderStatus.StockConfirmed;
        }
    }

    public void SetPaidStatus()
    {
        if (Status is OrderStatus.StockConfirmed)
        {
            Status = OrderStatus.Paid;
        }
    }

    public void SetCompletedStatus()
    {
        if (Status is not OrderStatus.Paid)
        {
            throw new InvalidOperationException("Order can only be completed if it is in Paid status.");
        }

        Status = OrderStatus.Completed;
    }

    public void SetCancelledStatus()
    {
        if (Status is OrderStatus.Paid or OrderStatus.Completed)
        {
            throw new InvalidOperationException("Paid or Completed orders cannot be cancelled.");
        }

        Status = OrderStatus.Cancelled;
    }
}