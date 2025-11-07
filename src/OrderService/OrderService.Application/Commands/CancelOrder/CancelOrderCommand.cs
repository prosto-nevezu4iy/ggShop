using OrderService.Application.Interfaces;

namespace OrderService.Application.Commands.CancelOrder;

public record CancelOrderCommand(Guid OrderId, Guid UserId) : ICommand<bool>;