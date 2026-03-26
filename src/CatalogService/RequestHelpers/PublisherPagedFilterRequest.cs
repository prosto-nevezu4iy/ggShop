using CatalogService.Enums;
using Common.Application.Requests.Filtering;
using Common.Application.Requests.Pagination;
using Common.Application.Requests.Sorting;

namespace CatalogService.RequestHelpers;

public record PublisherPagedFilterRequest(string SearchTerm, PublisherSortOption? Sort)
    : PagedRequest, IFilterRequest, ISortRequest<PublisherSortOption>;