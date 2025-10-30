using OrderService.Application.DTOs;
using OrderService.Application.Interfaces;

namespace OrderService.Application.Commands.CreateOrder;

public record CreateOrderCommand : ICommand<OrderDto>;