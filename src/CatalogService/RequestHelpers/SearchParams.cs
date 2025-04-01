namespace CatalogService.RequestHelpers;

public record SearchParams(
    string SearchTerm,
    int? FromPrice,
    int? ToPrice,
    bool? HasDiscount,
    Guid[] Genres,
    Guid[] Platforms,
    Guid? Publisher,
    bool? IsAvailable,
    string OrderBy,
    int PageSize = 3,
    int PageIndex = 0);
