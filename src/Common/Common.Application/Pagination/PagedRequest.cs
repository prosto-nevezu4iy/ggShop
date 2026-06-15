namespace Common.Application.Pagination;

public abstract record PagedRequest
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 3;

    public int GetOffset() => (PageNumber - 1) * PageSize;
}