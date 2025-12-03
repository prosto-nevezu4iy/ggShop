using CatalogService.Enums;
using Common.Application.Requests.Filtering;
using Common.Application.Requests.Pagination;
using Common.Application.Requests.Sorting;

namespace CatalogService.RequestHelpers;

public record GamePagedFilterRequest(
    string SearchTerm,
    decimal? FromPrice,
    decimal? ToPrice,
    bool? HasDiscount,
    Guid[] Genres,
    Guid[] Platforms,
    Guid? Publisher,
    bool? IsAvailable,
    SortRequest<GameSortOption> Sort) : PagedRequest, IFilterRequest;