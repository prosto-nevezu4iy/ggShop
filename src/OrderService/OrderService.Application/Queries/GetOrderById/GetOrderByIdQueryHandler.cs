using Microsoft.EntityFrameworkCore;
using OrderService.Application.DTOs;
using OrderService.Application.Interfaces;
using OrderService.Infrastructure;

namespace OrderService.Application.Queries.GetOrderById;

public class GetOrderByIdQueryHandler : IQueryHandler<GetOrderByIdQuery, OrderDto>
{
    private readonly OrderingContext _dbContext;

    public GetOrderByIdQueryHandler(OrderingContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<OrderDto> Handle(GetOrderByIdQuery query, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == query.OrderId && o.UserId == query.UserId, cancellationToken);

        ArgumentNullException.ThrowIfNull(order);

        return new OrderDto
        {
            Id = order.Id,
            TotalAmount = order.GetTotalAmount(),
            Status = order.Status.ToString(),
            OrderDate = order.OrderDate,
            OrderItems = order.OrderItems.Select(i => new OrderItemDto
            {
                GameId = i.GameId,
                Name = i.Name,
                Price = i.Price,
                Quantity = i.Quantity,
                ImageUrl = i.ImageUrl
            }).ToList()
        };
    }
}