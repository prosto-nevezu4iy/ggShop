using CatalogService.Abstractions;
using CatalogService.Entities;
using CatalogService.Enums;

namespace CatalogService.Services.Publishers;

public class PublisherOrderBuilder : IOrderBuilder<Publisher, PublisherSortOption>
{
    public IQueryable<Publisher> Build(IQueryable<Publisher> query, PublisherSortOption? order)
    {
        order ??= PublisherSortOption.Az;

        return order.Value switch
        {
            PublisherSortOption.Az => query.OrderBy(g => g.Name),
            PublisherSortOption.Za => query.OrderByDescending(g => g.Name),
            _ => query.OrderBy(g => g.Name)
        };
    }
}