using Microsoft.EntityFrameworkCore;
using OrderService.Application.DTOs;
using OrderService.Application.Interfaces;
using OrderService.Infrastructure;

namespace OrderService.Application.Queries.GetOrdersByUser;

public class GetOrdersByUserQueryHandler : IQueryHandler<GetOrdersByUserQuery, IEnumerable<OrderDto>>
{
    private readonly OrderingContext _dbContext;

    public GetOrdersByUserQueryHandler(OrderingContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<OrderDto>> Handle(GetOrdersByUserQuery query, CancellationToken cancellationToken)
    {
        var orders =  await _dbContext.Orders.Where(o => o.UserId == query.UserId).ToListAsync(cancellationToken: cancellationToken);

        return orders.Select(order => new OrderDto
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
        });
    }
}