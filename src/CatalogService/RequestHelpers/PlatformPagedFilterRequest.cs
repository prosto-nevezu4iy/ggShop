using CatalogService.Enums;
using Common.Application.Pagination;
using Common.Application.Pagination.Filtering;
using Common.Application.Pagination.Sorting;

namespace CatalogService.RequestHelpers;

public record PlatformPagedFilterRequest(string SearchTerm, PlatformSortOption? Sort)
    : PagedRequest, IFilterRequest, ISortRequest<PlatformSortOption>;