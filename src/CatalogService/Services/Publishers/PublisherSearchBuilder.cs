using CatalogService.Abstractions;
using CatalogService.Entities;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Services.Publishers;

public class PublisherSearchBuilder : ISearchBuilder<Publisher>
{
    public IQueryable<Publisher> Build(IQueryable<Publisher> query, string searchTerm)
    {
        if (string.IsNullOrEmpty(searchTerm))
        {
            return query;
        }

        return query.Where(g => EF.Functions.ILike(g.Name, $"%{searchTerm}%"));
    }
}