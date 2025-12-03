using CatalogService.DTOs;
using CatalogService.Entities;
using CatalogService.RequestHelpers;
using Common.Application.Requests.Pagination;
using Common.Domain;

namespace CatalogService.Abstractions;

public interface IGameService
{
    Task<PaginatedItems<GameDto>> GetGamesAsync(GamePagedFilterRequest request);
    Task<Result<GameDto>> GetGameByIdAsync(Guid id);
    Task<Game> GetGameEntityByIdAsync(Guid id);
    Task<Result<GameDto>> CreateGameAsync(CreateGameDto createGameDto);
    Task<Result> UpdateGameAsync(Guid id, UpdateGameDto updateGameDto);
    Task UpdateImageUrlAsync(Game game, string imageUrl);
    Task UpdateScreenShotUrlsAsync(Game game, ICollection<string> screenShotUrls);
    Task<Result> DeleteGameAsync(Guid id);
}
