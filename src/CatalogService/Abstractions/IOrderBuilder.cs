using CatalogService.Enums;

namespace CatalogService.Abstractions;

public interface IOrderBuilder<TEntity> where TEntity : class
{
    IQueryable<TEntity> Build(IQueryable<TEntity> query, GameSortOption? order);
}