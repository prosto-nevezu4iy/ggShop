namespace Common.Application.Requests.Pagination;

public class PaginatedItems<TEntity>(int pageIndex, int pageSize, int totalCount, IEnumerable<TEntity> results) where TEntity : class
{
    public int PageIndex { get; } = pageIndex;

    public int PageSize { get; } = pageSize;

    public int TotalCount { get; } = totalCount;

    public IEnumerable<TEntity> Results { get; } = results;
}
