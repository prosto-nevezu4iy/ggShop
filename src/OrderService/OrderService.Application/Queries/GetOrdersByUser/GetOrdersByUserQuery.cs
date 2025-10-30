using OrderService.Application.DTOs;
using OrderService.Application.Interfaces;

namespace OrderService.Application.Queries.GetOrdersByUser;

public record GetOrdersByUserQuery(Guid UserId) : IQuery<IEnumerable<OrderDto>>;