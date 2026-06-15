using CatalogService.Enums;
using Common.Application.Pagination;
using Common.Application.Pagination.Filtering;
using Common.Application.Pagination.Sorting;

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
    GameSortOption? Sort) : PagedRequest, IFilterRequest, ISortRequest<GameSortOption>;
