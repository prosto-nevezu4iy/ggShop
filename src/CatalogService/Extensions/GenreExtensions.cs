using CatalogService.DTOs.Genres;
using CatalogService.Entities;

namespace CatalogService.Extensions;

public static class GenreExtensions
{
    public static GenreDto ToDto(this Genre genre)
    {
        return new GenreDto(genre.Id, genre.Name);
    }

    public static Genre ToEntity(this CreateGenreDto createGenreDto)
    {
        return new Genre
        {
            Name = createGenreDto.Name
        };
    }
}