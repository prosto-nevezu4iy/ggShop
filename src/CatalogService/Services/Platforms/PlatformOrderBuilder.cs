using CatalogService.Abstractions;
using CatalogService.Entities;
using CatalogService.Enums;

namespace CatalogService.Services.Platforms;

public class PlatformOrderBuilder : IOrderBuilder<Platform, PlatformSortOption>
{
    public IQueryable<Platform> Build(IQueryable<Platform> query, PlatformSortOption? order)
    {
        order ??= PlatformSortOption.Az;

        return order.Value switch
        {
            PlatformSortOption.Az => query.OrderBy(g => g.Name),
            PlatformSortOption.Za => query.OrderByDescending(g => g.Name),
            _ => query.OrderBy(g => g.Name)
        };
    }
}