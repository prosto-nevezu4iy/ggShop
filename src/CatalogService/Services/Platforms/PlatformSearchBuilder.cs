using CatalogService.Abstractions;
using CatalogService.Entities;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Services.Platforms;

public class PlatformSearchBuilder : ISearchBuilder<Platform>
{
    public IQueryable<Platform> Build(IQueryable<Platform> query, string searchTerm)
    {
        if (string.IsNullOrEmpty(searchTerm))
        {
            return query;
        }

        return query.Where(g => EF.Functions.ILike(g.Name, $"%{searchTerm}%"));
    }
}