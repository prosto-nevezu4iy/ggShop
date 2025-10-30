using OrderService.Application.DTOs;
using OrderService.Application.Interfaces;
using OrderService.Application.Services;
using OrderService.Infrastructure;

namespace OrderService.Application.Commands.CreateOrder;

public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, OrderDto>
{
    private readonly OrderingContext _dbContext;
    private readonly GrpcShoppingCartClient _grpcShoppingCartClient;

    public CreateOrderCommandHandler(OrderingContext dbContext, GrpcShoppingCartClient grpcShoppingCartClient)
    {
        _dbContext = dbContext;
        _grpcShoppingCartClient = grpcShoppingCartClient;
    }

    public async Task<OrderDto> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var shoppingCart = await _grpcShoppingCartClient.GetShoppingCart();

        if (shoppingCart == null || !shoppingCart.Items.Any())
        {
            // TODO: How to handle this?
            return new OrderDto();
        }

        // var order = new Order(command.UserId);
        //
        // foreach (var item in command.Items)
        // {
        //     order.AddOrderItem(item.GameId, item.Name, item.Price, item.Quantity, item.ImageUrl);
        // }
        //
        // await _dbContext.Orders.AddAsync(order, cancellationToken);
        // await _dbContext.SaveChangesAsync(cancellationToken);
        //
        // return order;

        return new OrderDto();
    }
}