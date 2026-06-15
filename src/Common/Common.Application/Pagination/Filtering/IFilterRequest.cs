namespace Common.Application.Pagination.Filtering;

public interface IFilterRequest
{
    string SearchTerm { get; init; }
}