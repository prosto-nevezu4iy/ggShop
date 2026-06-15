using CatalogService.DTOs.Genres;
using CatalogService.RequestHelpers;
using Common.Application.Pagination;
using Common.Domain;

namespace CatalogService.Abstractions.Genres;

public interface IGenreService
{
    Task<Result<PaginatedItems<GenreDto>>> GetGenresAsync(GenrePagedFilterRequest request);
    Task<Result<GenreDto>> GetGenreByIdAsync(Guid id);
    Task<Result<GenreDto>> CreateGenreAsync(CreateGenreDto createGenreDto);
    Task<Result> UpdateGenreAsync(Guid id, UpdateGenreDto updateGenreDto);
    Task<Result> DeleteGenreAsync(Guid id);
}