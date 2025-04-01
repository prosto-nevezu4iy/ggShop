using CatalogService.DTOs;
using CatalogService.Entities;
using CatalogService.RequestHelpers;

namespace CatalogService.Services;

public interface ICatalogService
{
    Task<Guid> CreateGameAsync(CreateGameDto createGameDto);
    Task<GameDto> GetGameByIdAsync(Guid id);
    Task<Game> GetGameEntityByIdAsync(Guid id);
    Task<Game> GetGameEntityWithPlatformsByIdAsync(Guid id);
    Task<PaginatedItems<GameDto>> GetGamesAsync(SearchParams request);
    Task UpdateGameAsync(Game game, UpdateGameDto updateGameDto);
    Task UpdateImageUrlAsync(Game game, string imageUrl);
    Task UpdateScreenShotUrlsAsync(Game game, ICollection<string> screenShotUrls);
    Task DeleteGameAsync(Game game);
}
