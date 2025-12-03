using CatalogService.DTOs;
using CatalogService.Entities;

namespace CatalogService.Extensions;

public static class GameExtensions
{
    public static Game ToEntity(
        this CreateGameDto game,
        Publisher publisher,
        IReadOnlyCollection<Genre> genres,
        IReadOnlyCollection<Platform> platforms)
    {
        return new Game
        {
            Name = game.Name,
            Description = game.Description,
            FullDescription = game.FullDescription,
            SystemRequirements = game.SystemRequirements,
            Price = game.Price,
            Discount = game.Discount,
            AvailableStock = game.AvailableStock,
            ReleaseDate = game.ReleaseDate,
            Rating = game.Rating,
            IsPreOrder = game.IsPreOrder,
            ImageUrl = game.ImageUrl,
            TrailerUrl = game.TrailerUrl,
            BackgroundUrl = game.BackgroundUrl,
            Publisher = publisher,
            ScreenShotUrls = game.ScreenShotUrls.ToList(),
            Genres = genres.ToList(),
            Platforms = platforms.ToList()
        };
    }

    public static GameDto ToDto(this Game game)
    {
        return new GameDto(
            game.Id,
            game.Name,
            game.Description,
            game.FullDescription,
            game.SystemRequirements,
            game.Price,
            game.Discount,
            game.DiscountPrice,
            game.AvailableStock,
            new PublisherDto(game.Publisher.Id, game.Publisher.Name),
            game.ReleaseDate,
            game.Rating,
            game.AverageUserRating,
            game.TotalUserRatings,
            game.IsPreOrder,
            game.ImageUrl,
            game.TrailerUrl,
            game.BackgroundUrl,
            game.ScreenShotUrls.ToList(),
            game.Genres.Select(g => new GenreDto(g.Id, g.Name)).ToList(),
            game.Platforms.Select(p => new PlatformDto(p.Id, p.Name)).ToList());
    }

    public static GetGamesResponse ToGetGamesResponse(this IReadOnlyCollection<Game> games)
    {
        var response = new GetGamesResponse();

        response.Items.AddRange(games.Select(x => new GameItem
        {
            GameId = x.Id.ToString(),
            Name = x.Name,
            Price = (double)x.Price,
            AvailableStock = x.AvailableStock,
            ImageUrl = x.ImageUrl
        }));

        return response;
    }
}