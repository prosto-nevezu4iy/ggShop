using OrderService.Application.DTOs;
using OrderService.Application.Extensions;
using OrderService.Application.Interfaces;
using OrderService.Application.Services;
using OrderService.Domain.Entities;
using OrderService.Infrastructure;

namespace OrderService.Application.Commands.CreateOrder;

public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, OrderDto>
{
    private readonly OrderingContext _dbContext;
    private readonly GrpcShoppingCartClient _grpcShoppingCartClient;
    private readonly GrpcCatalogClient _grpcCatalogClient;

    public CreateOrderCommandHandler(OrderingContext dbContext, GrpcShoppingCartClient grpcShoppingCartClient, GrpcCatalogClient grpcCatalogClient)
    {
        _dbContext = dbContext;
        _grpcShoppingCartClient = grpcShoppingCartClient;
        _grpcCatalogClient = grpcCatalogClient;
    }

    public async Task<OrderDto> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var shoppingCart = await _grpcShoppingCartClient.GetShoppingCartAsync(command.Token);

        if (shoppingCart == null || !shoppingCart.Items.Any())
        {
            // TODO: How to handle this?
            return new OrderDto();
        }

        var catalog = await _grpcCatalogClient.GetGamesAsync(shoppingCart.Items.Select(x => x.GameId).ToList());

        foreach (var item in shoppingCart.Items)
        {
            var game = catalog.Items.FirstOrDefault(g => g.GameId == item.GameId);

            if (game!.AvailableStock < item.Quantity)
            {
                //TODO: How to handle this?
                // return Result<Guid>.Failure("OutOfStock", $"Not enough stock for {game.Name}.");
            }
        }

        var order = new Order(command.UserId);

        foreach (var item in shoppingCart.Items)
        {
            var game = catalog.Items.FirstOrDefault(g => g.GameId == item.GameId);
            order.AddOrderItem(item.GameId, game!.Name, game.Price, item.Quantity, game.ImageUrl);
        }

        await _dbContext.Orders.AddAsync(order, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return order.ToOrderDto();
    }
}