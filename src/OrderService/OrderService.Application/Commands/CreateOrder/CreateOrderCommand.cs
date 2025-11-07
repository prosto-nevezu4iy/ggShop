using Microsoft.AspNetCore.Http;
using OrderService.Application.DTOs;
using OrderService.Application.Interfaces;

namespace OrderService.Application.Commands.CreateOrder;

public record CreateOrderCommand(string Token, Guid UserId) : ICommand<OrderDto>;