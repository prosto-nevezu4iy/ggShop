using CatalogService.Enums;
using Common.Application.Requests.Filtering;
using Common.Application.Requests.Pagination;
using Common.Application.Requests.Sorting;

namespace CatalogService.RequestHelpers;

public record PlatformPagedFilterRequest(string SearchTerm, PlatformSortOption? Sort)
    : PagedRequest, IFilterRequest, ISortRequest<PlatformSortOption>;