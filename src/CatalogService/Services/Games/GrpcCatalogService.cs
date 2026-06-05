using CatalogService.Extensions;
using CatalogService.Infrastructure;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Services.Games;

public class GrpcCatalogService(CatalogContext dbContext, ILogger<GrpcCatalogService> logger)
    : GrpcCatalog.GrpcCatalogBase
{
    public override async Task<GetGamesResponse> GetGames(GetGamesRequest request, ServerCallContext context)
    {
        logger.LogDebug("Begin GetGames call from method {Method}", context.Method);

        var gameIds = request.Items.Select(i => Guid.Parse(i.GameId)).ToList();

        var games = await dbContext.Games
            .Where(g => gameIds.Contains(g.Id))
            .ToListAsync();

        return games is { Count: > 0 } ? games.ToGetGamesResponse() : new GetGamesResponse();
    }
}