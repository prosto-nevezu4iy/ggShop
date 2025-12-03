namespace Common.Application.Requests.Filtering;

public interface IFilterRequest
{
    string SearchTerm { get; init; }
}