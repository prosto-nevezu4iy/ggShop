using System.Linq.Expressions;
using Common.Application.Requests.Filtering;

namespace CatalogService.Abstractions;

public abstract class FilterBuilder<TEntity, TRequest>
where TEntity : class
where TRequest : IFilterRequest
{
    private IQueryable<TEntity> _query;

    public FilterBuilder<TEntity, TRequest> Init(IQueryable<TEntity> baseQuery)
    {
        _query = baseQuery;

        return this;
    }

    public FilterBuilder<TEntity, TRequest> WhereIf(bool condition, Expression<Func<TEntity, bool>> predicate)
    {
        if (condition)
        {
            _query = _query.Where(predicate);
        }

        return this;
    }

    public abstract FilterBuilder<TEntity, TRequest> ApplyFilters(TRequest request);

    public IQueryable<TEntity> Build()
    {
        return _query;
    }
}