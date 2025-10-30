using OrderService.Application.DTOs;
using OrderService.Application.Interfaces;

namespace OrderService.Application.Queries.GetOrderById;

public record GetOrderByIdQuery(Guid OrderId, Guid UserId) : IQuery<OrderDto>;