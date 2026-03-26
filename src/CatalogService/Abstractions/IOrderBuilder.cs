namespace CatalogService.Abstractions;

public interface IOrderBuilder<TEntity, TSortOption>
    where TEntity : class
    where TSortOption : struct, Enum
{
    IQueryable<TEntity> Build(IQueryable<TEntity> query, TSortOption? order);
}