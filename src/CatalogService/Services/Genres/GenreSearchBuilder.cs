using CatalogService.Abstractions;
using CatalogService.Entities;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Services.Genres;

public class GenreSearchBuilder : ISearchBuilder<Genre>
{
    public IQueryable<Genre> Build(IQueryable<Genre> query, string searchTerm)
    {
        if (string.IsNullOrEmpty(searchTerm))
        {
            return query;
        }

        return query.Where(g => EF.Functions.ILike(g.Name, $"%{searchTerm}%"));
    }
}