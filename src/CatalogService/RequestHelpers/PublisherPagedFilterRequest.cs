using CatalogService.Enums;
using Common.Application.Pagination;
using Common.Application.Pagination.Filtering;
using Common.Application.Pagination.Sorting;

namespace CatalogService.RequestHelpers;

public record PublisherPagedFilterRequest(string SearchTerm, PublisherSortOption? Sort)
    : PagedRequest, IFilterRequest, ISortRequest<PublisherSortOption>;