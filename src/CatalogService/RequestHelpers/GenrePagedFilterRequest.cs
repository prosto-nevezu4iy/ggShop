using CatalogService.Enums;
using Common.Application.Pagination;
using Common.Application.Pagination.Filtering;
using Common.Application.Pagination.Sorting;

namespace CatalogService.RequestHelpers;

public record GenrePagedFilterRequest(string SearchTerm, GenreSortOption? Sort)
    : PagedRequest, IFilterRequest, ISortRequest<GenreSortOption>;