namespace CatalogService.Abstractions;

public interface ISearchBuilder<T> where T : class
{
    IQueryable<T> Build(IQueryable<T> query, string searchTerm);
}