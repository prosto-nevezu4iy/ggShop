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

public class GenreService(
    CatalogContext dbContext,
    IValidator<GenrePagedFilterRequest> genrePagedFilterValidator,
    IValidator<CreateGenreDto> createValidator,
    IValidator<UpdateGenreDto> updateValidator,
    ISearchBuilder<Genre> searchBuilder,
    IOrderBuilder<Genre, GenreSortOption> orderBuilder)
    : IGenreService
{
    public async Task<Result<PaginatedItems<GenreDto>>> GetGenresAsync(GenrePagedFilterRequest request)
    {
        var validationResult = await genrePagedFilterValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return new ValidationError(validationResult.ToErrorDictionary());
        }

        var query = dbContext.Genres.AsNoTracking();

        query = searchBuilder.Build(query, request.SearchTerm);

        query = orderBuilder.Build(query, request.Sort);

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
        var genre = await dbContext.Genres.AsNoTracking().SingleOrDefaultAsync(g => g.Id == id);

        return genre is null
            ? GenreErrors.NotFound(id)
            : genre.ToDto();
    }

    public async Task<Result<GenreDto>> CreateGenreAsync(CreateGenreDto createGenreDto)
    {
        var validationResult = await createValidator.ValidateAsync(createGenreDto);

        if (!validationResult.IsValid)
        {
            return new ValidationError(validationResult.ToErrorDictionary());
        }

        var genre = createGenreDto.ToEntity();

        await dbContext.Genres.AddAsync(genre);

        var result = await dbContext.SaveChangesAsync() > 0;

        if (!result)
        {
            return GenreErrors.GenreNotCreated;
        }

        return genre.ToDto();
    }

    public async Task<Result> UpdateGenreAsync(Guid id, UpdateGenreDto updateGenreDto)
    {
        var validationResult = await updateValidator.ValidateAsync(updateGenreDto);

        if (!validationResult.IsValid)
        {
            return new ValidationError(validationResult.ToErrorDictionary());
        }

        var genre = await dbContext.Genres.SingleOrDefaultAsync(g => g.Id == id);

        if (genre is null)
        {
            return GenreErrors.NotFound(id);
        }

        dbContext.Entry(genre).CurrentValues.SetValues(updateGenreDto);

        var result = await dbContext.SaveChangesAsync() > 0;

        if (!result)
        {
            return GenreErrors.GenreNotUpdated;
        }

        return Result.Success();
    }

    public async Task<Result> DeleteGenreAsync(Guid id)
    {
        var genre = await dbContext.Genres.SingleOrDefaultAsync(g => g.Id == id);

        if (genre is null)
        {
            return GenreErrors.NotFound(id);
        }

        dbContext.Genres.Remove(genre);

        var result = await dbContext.SaveChangesAsync() > 0;

        if (!result)
        {
            return GenreErrors.GenreNotDeleted;
        }

        return Result.Success();
    }
}