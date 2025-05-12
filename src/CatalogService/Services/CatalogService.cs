using AutoMapper;
using AutoMapper.QueryableExtensions;
using CatalogService.DTOs;
using CatalogService.Entities;
using CatalogService.Infrastructure;
using CatalogService.RequestHelpers;
using Contracts;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Services;

public class CatalogService : ICatalogService
{
    private readonly CatalogContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IImageService _imageService;
    private readonly IPublishEndpoint _publishEndpoint;

    public CatalogService(CatalogContext dbContext, IMapper mapper, IImageService imageService, IPublishEndpoint publishEndpoint)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _imageService = imageService;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Guid> CreateGameAsync(CreateGameDto createGameDto)
    {
        var game = _mapper.Map<Game>(createGameDto);

        _dbContext.Genres.AttachRange(game.Genres);
        _dbContext.Platforms.AttachRange(game.Platforms);
        _dbContext.Publishers.Attach(game.Publisher);
        await _dbContext.Games.AddAsync(game);

        // Publish event

        await _dbContext.SaveChangesAsync();

        await _imageService.UploadImageJob(game.Id, createGameDto.ImageUrl);
        await _imageService.UploadScreenShotsJob(game.Id, createGameDto.ScreenShotUrls);

        return game.Id;
    }

    public async Task<GameDto> GetGameByIdAsync(Guid id)
    {
        return await _dbContext.Games.Include(x => x.Platforms)
            .Include(x => x.Genres)
            .ProjectTo<GameDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Game> GetGameEntityByIdAsync(Guid id)
    {
        return await _dbContext.Games
            .SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Game> GetGameEntityWithPlatformsByIdAsync(Guid id)
    {
        return await _dbContext.Games
           .Include(x => x.Platforms)
           .SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task<PaginatedItems<GameDto>> GetGamesAsync(SearchParams request)
    {
        var query = _dbContext.Games.AsNoTracking();

        query = SearchGames(query, request.SearchTerm);

        query = FilterGames(query, request);

        query = OrderGames(query, request);

        var totalItems = await query.CountAsync();

        var games = await query
            .Include(g => g.Platforms)
            .Include(g => g.Genres)
            .Skip(request.PageSize * request.PageIndex)
            .Take(request.PageSize)
            .ProjectTo<GameDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return new PaginatedItems<GameDto>(request.PageIndex, request.PageSize, totalItems, games);
    }

    public async Task UpdateGameAsync(Game game, UpdateGameDto updateGameDto)
    {
        var oldScreenShotUrls = game.ScreenShotUrls;

        _dbContext.Entry(game).CurrentValues.SetValues(updateGameDto);

        // Get new platform IDs
        var newPlatformIds = updateGameDto.Platforms;

        // Get current platform IDs
        var currentPlatformIds = game.Platforms.Select(p => p.Id).ToList();

        // Find platforms to remove and platforms to add
        var platformIdsToRemove = currentPlatformIds.Except(newPlatformIds).ToList();
        var platformIdsToAdd = newPlatformIds.Except(currentPlatformIds).ToList();

        game.Platforms.RemoveAll(p => platformIdsToRemove.Contains(p.Id));

        // Add new platforms
        if (platformIdsToAdd.Any())
        {
            var platformsToAdd = await _dbContext.Platforms
                .Where(p => platformIdsToAdd.Contains(p.Id))
                .ToListAsync();

            game.Platforms.AddRange(platformsToAdd);
        }

        // Publish event

        await _dbContext.SaveChangesAsync();
        await _imageService.DeleteScreenShotsJob(game.Id, oldScreenShotUrls);
        await _imageService.UploadScreenShotsJob(game.Id, updateGameDto.ScreenShotUrls);
    }

    public async Task UpdateImageUrlAsync(Game game, string imageUrl)
    {
        game.ImageUrl = imageUrl;
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateScreenShotUrlsAsync(Game game, ICollection<string> imageUrl)
    {
        game.ScreenShotUrls = imageUrl;
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteGameAsync(Game game)
    {
        _dbContext.Games.Remove(game);

        await _publishEndpoint.Publish<GameDeleted>((new {Id = game.Id}));
        
        await _dbContext.SaveChangesAsync();
        
        await _imageService.DeleteImageJob(game.Id, game.ImageUrl);
        await _imageService.DeleteScreenShotsJob(game.Id, game.ScreenShotUrls);
    }

    private IQueryable<Game> SearchGames(IQueryable<Game> query, string searchTerm)
    {
        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query
                    .Where(g =>
                        g.SearchVector.Matches(EF.Functions.WebSearchToTsQuery("english", searchTerm)))
                    .OrderByDescending(g =>
                        g.SearchVector.Rank(EF.Functions.WebSearchToTsQuery("english", searchTerm)));
        }

        return query;
    }

    private IQueryable<Game> FilterGames(IQueryable<Game> query, SearchParams request)
    {
        if (request.FromPrice.HasValue)
        {
            query = query.Where(g => g.DiscountPrice >= request.FromPrice);
        }

        if (request.ToPrice.HasValue)
        {
            query = query.Where(g => g.DiscountPrice <= request.ToPrice);
        }

        if (request.HasDiscount.HasValue)
        {
            query = query.Where(g => g.Discount > 0);
        }

        if (request.Genres.Any())
        {
            query = query.Where(g => g.Genres.Any(gr => request.Genres.Contains(gr.Id)));
        }

        if (request.Platforms.Any())
        {
            query = query.Where(g => g.Platforms.Any(gr => request.Platforms.Contains(gr.Id)));
        }

        if (request.Publisher.HasValue)
        {
            query = query.Where(g => g.Publisher.Id == request.Publisher);
        }

        if (request.IsAvailable.HasValue)
        {
            query = query.Where(g => g.AvailableStock > 0);
        }

        return query;
    }

    private IQueryable<Game> OrderGames(IQueryable<Game> query, SearchParams request)
    {
        query = request.OrderBy switch
        {
            "popular" => query.OrderBy(g => g.AvailableStock),
            "new" => query.OrderByDescending(g => g.ReleaseDate),
            "cheap" => query.OrderBy(g => g.Price),
            "expensive" => query.OrderByDescending(g => g.Price),
            "discount" => query.OrderByDescending(g => g.Discount),
            _ => query.OrderBy(g => g.AvailableStock)
        };

        return query;
    }
}
