namespace Common.Application.Requests.Pagination;

public abstract record PagedRequest(int PageIndex = 0, int PageSize = 3);