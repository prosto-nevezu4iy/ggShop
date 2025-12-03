using CatalogService.Abstractions;
using CatalogService.Entities;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Services;

public class GameSearchBuilder : ISearchBuilder<Game>
{
    public IQueryable<Game> Build(IQueryable<Game> query, string searchTerm)
    {
        if (string.IsNullOrEmpty(searchTerm))
        {
            return query;
        }

        return query
            .Where(g =>
                g.SearchVector.Matches(EF.Functions.WebSearchToTsQuery("english", searchTerm)))
            .OrderByDescending(g =>
                g.SearchVector.Rank(EF.Functions.WebSearchToTsQuery("english", searchTerm)));
    }
}