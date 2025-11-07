using CatalogService.Entities;
using CatalogService.Infrastructure;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Services;

public class GrpcCatalogService : GrpcCatalog.GrpcCatalogBase
{
    private readonly CatalogContext _dbContext;
    private readonly ILogger<GrpcCatalogService> _logger;

    public GrpcCatalogService(CatalogContext dbContext, ILogger<GrpcCatalogService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public override async Task<GetGamesResponse> GetGames(GetGamesRequest request, ServerCallContext context)
    {
        _logger.LogDebug("Begin GetGames call from method {Method}", context.Method);

        var gameIds = request.Items.Select(i => Guid.Parse(i.GameId)).ToList();

        var games = await _dbContext.Games
            .Where(g => gameIds.Contains(g.Id))
            .ToListAsync();

        if (games.Any())
        {
            return MapToGetGameResponse(games);
        }

        return new GetGamesResponse();
    }

    private static GetGamesResponse MapToGetGameResponse(IEnumerable<Game> games)
    {
        var response = new GetGamesResponse();

        foreach (var item in games)
        {
            response.Items.Add(new GameItem
            {
                GameId = item.Id.ToString(),
                Name = item.Name,
                Price = (double)item.Price,
                AvailableStock = item.AvailableStock,
                ImageUrl = item.ImageUrl
            });
        }

        return response;
    }
}