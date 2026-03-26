using CatalogService.Abstractions;
using CatalogService.Abstractions.Genres;
using CatalogService.DTOs.Genres;
using CatalogService.Entities;
using CatalogService.Enums;
using CatalogService.Errors;
using CatalogService.Extensions;
using CatalogService.Infrastructure;
using CatalogService.RequestHelpers;
using Common.Application.Requests.Pagination;
using Common.Domain;
using Common.Presentation.Extensions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Services.Genres;

public class GenreService : IGenreService
{
    private readonly CatalogContext _dbContext;
    private readonly IValidator<GenrePagedFilterRequest> _genrePagedFilterValidator;
    private readonly IValidator<CreateGenreDto> _createValidator;
    private readonly IValidator<UpdateGenreDto> _updateValidator;
    private readonly ISearchBuilder<Genre> _searchBuilder;
    private readonly IOrderBuilder<Genre, GenreSortOption> _orderBuilder;

    public GenreService(
        CatalogContext dbContext,
        IValidator<GenrePagedFilterRequest> genrePagedFilterValidator,
        IValidator<CreateGenreDto> createValidator,
        IValidator<UpdateGenreDto> updateValidator,
        ISearchBuilder<Genre> searchBuilder,
        IOrderBuilder<Genre, GenreSortOption> orderBuilder)
    {
        _dbContext = dbContext;
        _genrePagedFilterValidator = genrePagedFilterValidator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _searchBuilder = searchBuilder;
        _orderBuilder = orderBuilder;
    }

    public async Task<Result<PaginatedItems<GenreDto>>> GetGenresAsync(GenrePagedFilterRequest request)
    {
        var validationResult = await _genrePagedFilterValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return new ValidationError(validationResult.ToErrorDictionary());
        }

        var query = _dbContext.Genres.AsNoTracking();

        query = _searchBuilder.Build(query, request.SearchTerm);

        query = _orderBuilder.Build(query, request.Sort);

        var totalItems = await query.CountAsync();

        var genres = await query
            .Skip(request.GetOffset())
            .Take(request.PageSize)
            .Select(x => x.ToDto())
            .ToListAsync();

        return new PaginatedItems<GenreDto>(request.PageNumber, request.PageSize, totalItems, genres);
    }

    public async Task<Result<GenreDto>> GetGenreByIdAsync(Guid id)
    {
        var genre = await _dbContext.Genres.AsNoTracking().SingleOrDefaultAsync(g => g.Id == id);

        return genre is null
            ? GenreErrors.NotFound(id)
            : genre.ToDto();
    }

    public async Task<Result<GenreDto>> CreateGenreAsync(CreateGenreDto createGenreDto)
    {
        var validationResult = await _createValidator.ValidateAsync(createGenreDto);

        if (!validationResult.IsValid)
        {
            return new ValidationError(validationResult.ToErrorDictionary());
        }

        var genre = createGenreDto.ToEntity();

        await _dbContext.Genres.AddAsync(genre);

        var result = await _dbContext.SaveChangesAsync() > 0;

        if (!result)
        {
            return GenreErrors.GenreNotCreated;
        }

        return genre.ToDto();
    }

    public async Task<Result> UpdateGenreAsync(Guid id, UpdateGenreDto updateGenreDto)
    {
        var validationResult = await _updateValidator.ValidateAsync(updateGenreDto);

        if (!validationResult.IsValid)
        {
            return new ValidationError(validationResult.ToErrorDictionary());
        }

        var genre = await _dbContext.Genres.SingleOrDefaultAsync(g => g.Id == id);

        if (genre is null)
        {
            return GenreErrors.NotFound(id);
        }

        _dbContext.Entry(genre).CurrentValues.SetValues(updateGenreDto);

        var result = await _dbContext.SaveChangesAsync() > 0;

        if (!result)
        {
            return GenreErrors.GenreNotUpdated;
        }

        return Result.Success();
    }

    public async Task<Result> DeleteGenreAsync(Guid id)
    {
        var genre = await _dbContext.Genres.SingleOrDefaultAsync(g => g.Id == id);

        if (genre is null)
        {
            return GenreErrors.NotFound(id);
        }

        _dbContext.Genres.Remove(genre);

        var result = await _dbContext.SaveChangesAsync() > 0;

        if (!result)
        {
            return GenreErrors.GenreNotDeleted;
        }

        return Result.Success();
    }
}