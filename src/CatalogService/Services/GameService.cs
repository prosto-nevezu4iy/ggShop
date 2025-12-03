using CatalogService.Abstractions;
using CatalogService.DTOs;
using CatalogService.Entities;
using CatalogService.Errors;
using CatalogService.Extensions;
using CatalogService.Infrastructure;
using CatalogService.RequestHelpers;
using Common.Application.Requests.Pagination;
using Common.Domain;
using Common.Presentation.Extensions;
using Contracts;
using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Services;

public class GameService : IGameService
{
    private readonly CatalogContext _dbContext;
    private readonly IValidator<CreateGameDto> _createValidator;
    private readonly IValidator<UpdateGameDto> _updateValidator;
    private readonly IJobService _jobService;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ISearchBuilder<Game> _searchBuilder;
    private readonly FilterBuilder<Game, GamePagedFilterRequest> _filterBuilder;
    private readonly IOrderBuilder<Game> _orderBuilder;

    public GameService(
        CatalogContext dbContext,
        IValidator<CreateGameDto> createValidator,
        IValidator<UpdateGameDto> updateValidator,
        IJobService jobService,
        IPublishEndpoint publishEndpoint,
        FilterBuilder<Game, GamePagedFilterRequest> filterBuilder,
        ISearchBuilder<Game> searchBuilder,
        IOrderBuilder<Game> orderBuilder)
    {
        _dbContext = dbContext;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _jobService = jobService;
        _publishEndpoint = publishEndpoint;
        _filterBuilder = filterBuilder;
        _searchBuilder = searchBuilder;
        _orderBuilder = orderBuilder;
    }

    public async Task<PaginatedItems<GameDto>> GetGamesAsync(GamePagedFilterRequest request)
    {
        var query = _dbContext.Games.AsNoTracking();

        query = _searchBuilder.Build(query, request.SearchTerm);

        query = _filterBuilder
            .Init(query)
            .ApplyFilters(request)
            .Build();

        query = _orderBuilder.Build(query, request.Sort?.Value);

        var totalItems = await query.CountAsync();

        var games = await query
            .Include(g => g.Platforms)
            .Include(g => g.Genres)
            .Include(g => g.Publisher)
            .Include(g => g.UserRatings)
            .Skip(request.PageSize * request.PageIndex)
            .Take(request.PageSize)
            .Select(x => x.ToDto())
            .ToListAsync();

        return new PaginatedItems<GameDto>(request.PageIndex, request.PageSize, totalItems, games);
    }

    public async Task<Result<GameDto>> GetGameByIdAsync(Guid id)
    {
        var game = await _dbContext.Games
            .AsNoTracking()
            .Include(x => x.Platforms)
            .Include(x => x.Genres)
            .Include(x => x.Publisher)
            .Include(x => x.UserRatings)
            .SingleOrDefaultAsync(x => x.Id == id);

        return game is null
            ? GameErrors.NotFound(id)
            : game.ToDto();
    }

    public async Task<Game> GetGameEntityByIdAsync(Guid id) =>
        await _dbContext.Games
            .SingleOrDefaultAsync(x => x.Id == id);

    public async Task<Result<GameDto>> CreateGameAsync(CreateGameDto createGameDto)
    {
        var validationResult = await _createValidator.ValidateAsync(createGameDto);

        if (!validationResult.IsValid)
        {
            return new ValidationError(validationResult.ToErrorDictionary());
        }

        var genres = await _dbContext.Genres
            .Where(x => createGameDto.Genres.Contains(x.Id))
            .ToListAsync();

        var platforms = await _dbContext.Platforms
            .Where(x => createGameDto.Platforms.Contains(x.Id))
            .ToListAsync();

        var publisher = await _dbContext.Publishers
            .SingleOrDefaultAsync(x => x.Id == createGameDto.Publisher);

        var game = createGameDto.ToEntity(publisher, genres, platforms);

        await _dbContext.Games.AddAsync(game);

        var result = await _dbContext.SaveChangesAsync() > 0;

        if (!result)
        {
            return GameErrors.GameNotCreated;
        }

        var uploadImageJobTask = _jobService.UploadImageJob(game.Id, createGameDto.ImageUrl);
        var uploadScreenShotsJobTask = _jobService.UploadScreenShotsJob(game.Id, createGameDto.ScreenShotUrls);

        await Task.WhenAll(uploadImageJobTask, uploadScreenShotsJobTask);

        return game.ToDto();
    }

    public async Task<Result> UpdateGameAsync(Guid id, UpdateGameDto updateGameDto)
    {
        var validationResult = await _updateValidator.ValidateAsync(updateGameDto);

        if (!validationResult.IsValid)
        {
            return new ValidationError(validationResult.ToErrorDictionary());
        }

        var game = await GetGameEntityWithPlatformsByIdAsync(id);

        if (game is null)
        {
            return GameErrors.NotFound(id);
        }

        _dbContext.Entry(game).CurrentValues.SetValues(updateGameDto);

        await UpdateGamePlatformsAsync(game, updateGameDto.Platforms);

        var result = await _dbContext.SaveChangesAsync() > 0;

        if (!result)
        {
            return GameErrors.GameNotUpdated;
        }

        var deleteScreenShotsJobTask = _jobService.DeleteScreenShotsJob(game.Id, game.ScreenShotUrls);
        var uploadScreenShotsJobTask = _jobService.UploadScreenShotsJob(game.Id, updateGameDto.ScreenShotUrls);

        await Task.WhenAll(deleteScreenShotsJobTask, uploadScreenShotsJobTask);

        return Result.Success();
    }

    public async Task UpdateImageUrlAsync(Game game, string imageUrl)
    {
        game.ImageUrl = imageUrl;
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateScreenShotUrlsAsync(Game game, ICollection<string> imageUrl)
    {
        game.ScreenShotUrls = imageUrl.ToList();
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Result> DeleteGameAsync(Guid id)
    {
        var game = await GetGameEntityByIdAsync(id);

        if (game is null)
        {
            return GameErrors.NotFound(id);
        }

        _dbContext.Games.Remove(game);

        await _publishEndpoint.Publish<GameDeleted>(new() {Id = game.Id});

        var result = await _dbContext.SaveChangesAsync() > 0;

        if (!result)
        {
            return GameErrors.GameNotDeleted;
        }

        var deleteImageJobTask = _jobService.DeleteImageJob(game.Id, game.ImageUrl);
        var deleteScreenShotsJobTask = _jobService.DeleteScreenShotsJob(game.Id, game.ScreenShotUrls);

        await Task.WhenAll(deleteImageJobTask, deleteScreenShotsJobTask);

        return Result.Success();
    }

    private async Task<Game> GetGameEntityWithPlatformsByIdAsync(Guid id)
    {
        return await _dbContext.Games
            .Include(x => x.Platforms)
            .SingleOrDefaultAsync(x => x.Id == id);
    }

    private async Task UpdateGamePlatformsAsync(Game game, IReadOnlyCollection<Guid> newPlatformIds)
    {
        // Get current platform IDs
        var currentPlatformIds = game.Platforms.Select(p => p.Id).ToList();

        // Find platforms to remove and platforms to add
        var platformIdsToRemove = currentPlatformIds.Except(newPlatformIds).ToList();
        var platformIdsToAdd = newPlatformIds.Except(currentPlatformIds).ToList();

        game.Platforms.RemoveAll(p => platformIdsToRemove.Contains(p.Id));

        // Add new platforms
        if (platformIdsToAdd is { Count: > 0 })
        {
            var platformsToAdd = await _dbContext.Platforms
                .Where(p => platformIdsToAdd.Contains(p.Id))
                .ToListAsync();

            game.Platforms.AddRange(platformsToAdd);
        }
    }
}
