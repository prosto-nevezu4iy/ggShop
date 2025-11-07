using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OrderService.Domain.Entities;
using ShoppingCartService;
using ShoppingCartItem = OrderService.Domain.Entities.ShoppingCartItem;

namespace OrderService.Application.Services;

public class GrpcShoppingCartClient(IConfiguration config, ILogger<GrpcShoppingCartClient> logger)
{
    public async Task<ShoppingCart> GetShoppingCartAsync(string token)
    {
        logger.LogInformation($"==> Calling GRPC Service: {config["GrpcShoppingCart"]}");

        var channel = GrpcChannel.ForAddress(config["GrpcShoppingCart"]!);
        var client = new GrpcShoppingCart.GrpcShoppingCartClient(channel);
        var request = new GetShoppingCartRequest();

        var headers = new Metadata
        {
            { "Authorization", $"Bearer {token}" }
        };

        try
        {
            var reply = await client.GetShoppingCartAsync(request, headers);

            var shoppingCartItems = reply.Items.Select(item => new ShoppingCartItem
            {
                GameId = Guid.Parse(item.GameId),
                Quantity = item.Quantity,
            }).ToList();

            return new ShoppingCart
            {
                Items = shoppingCartItems
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Could not call Grpc server");
            return null; // Return null if the shoppingCart cannot be retrieved
        }
    }
}