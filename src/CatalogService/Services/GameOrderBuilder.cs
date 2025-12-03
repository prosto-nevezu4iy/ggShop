using System.Linq.Expressions;
using CatalogService.Abstractions;
using CatalogService.Entities;
using CatalogService.Enums;

namespace CatalogService.Services;

public class GameOrderBuilder : IOrderBuilder<Game>
{
    public IQueryable<Game> Build(IQueryable<Game> query, GameSortOption? order)
    {
        order ??= GameSortOption.Popular;

        return order.Value switch
        {
            GameSortOption.Popular => query.OrderBy(g => g.AvailableStock),
            GameSortOption.New => query.OrderByDescending(g => g.ReleaseDate),
            GameSortOption.Cheap => query.OrderBy(g => g.DiscountPrice),
            GameSortOption.Expensive => query.OrderByDescending(g => g.DiscountPrice),
            GameSortOption.Discount => query.OrderByDescending(g => g.Discount),
            _ => query.OrderBy(g => g.AvailableStock)
        };
    }
}