namespace Common.Application.Requests.Sorting;

public interface ISortRequest<T> where T : struct, Enum
{
    T? Sort { get; init; }
}
