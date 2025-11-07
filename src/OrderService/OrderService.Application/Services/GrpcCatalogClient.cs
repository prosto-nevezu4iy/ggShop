using CatalogService;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OrderService.Domain.Entities;
using ShoppingCartItem = CatalogService.ShoppingCartItem;

namespace OrderService.Application.Services;

public class GrpcCatalogClient(IConfiguration configuration, ILogger<GrpcCatalogClient> logger)
{
    public async Task<Catalog> GetGamesAsync(IEnumerable<Guid> gameIds)
    {
        logger.LogInformation($"==> Calling Catalog Service: {configuration["GrpcCatalog"]}");

        var channel = GrpcChannel.ForAddress(configuration["GrpcCatalog"]!);
        var client = new GrpcCatalog.GrpcCatalogClient(channel);
        var request = new GetGamesRequest();
        request.Items.AddRange(gameIds.Select(id => new ShoppingCartItem { GameId = id.ToString() }));

        try
        {
            var reply = await client.GetGamesAsync(request);

            var shoppingCartItems = reply.Items.Select(item => new GameItem
            {
                GameId = Guid.Parse(item.GameId),
                Name = item.Name,
                Price = (decimal)item.Price,
                AvailableStock = item.AvailableStock,
                ImageUrl = item.ImageUrl
            }).ToList();

            return new Catalog()
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