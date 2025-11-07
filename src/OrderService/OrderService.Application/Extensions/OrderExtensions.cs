using OrderService.Application.DTOs;
using OrderService.Domain.Entities;

namespace OrderService.Application.Extensions;

public static class OrderExtensions
{
    public static OrderDto ToOrderDto(this Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            OrderDate = order.OrderDate,
            TotalAmount = order.GetTotalAmount(),
            Status = order.Status.ToString(),
            OrderItems = order.OrderItems.Select(item => new OrderItemDto
            {
                GameId = item.GameId,
                Name = item.Name,
                Price = item.Price,
                Quantity = item.Quantity,
                ImageUrl = item.ImageUrl,
            }).ToList()
        };
    }
}