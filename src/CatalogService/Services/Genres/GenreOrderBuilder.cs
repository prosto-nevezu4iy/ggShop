using CatalogService.Abstractions;
using CatalogService.Entities;
using CatalogService.Enums;

namespace CatalogService.Services.Genres;

public class GenreOrderBuilder : IOrderBuilder<Genre, GenreSortOption>
{
    public IQueryable<Genre> Build(IQueryable<Genre> query, GenreSortOption? order)
    {
        order ??= GenreSortOption.Az;

        return order.Value switch
        {
            GenreSortOption.Az => query.OrderBy(g => g.Name),
            GenreSortOption.Za => query.OrderByDescending(g => g.Name),
            _ => query.OrderBy(g => g.Name)
        };
    }
}