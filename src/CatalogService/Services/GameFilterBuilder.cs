using CatalogService.Abstractions;
using CatalogService.Entities;
using CatalogService.RequestHelpers;

namespace CatalogService.Services;

public class GameFilterBuilder : FilterBuilder<Game, GamePagedFilterRequest>
{
    public override FilterBuilder<Game, GamePagedFilterRequest> ApplyFilters(GamePagedFilterRequest request) =>
        WhereIf(request.FromPrice.HasValue, x => x.DiscountPrice >= request.FromPrice)
            .WhereIf(request.ToPrice.HasValue, x => x.DiscountPrice <= request.ToPrice)
            .WhereIf(request.HasDiscount is true, x => x.Discount > 0)
            .WhereIf(request.Genres is { Length: > 0 }, x => x.Genres.Any(g => request.Genres.Contains(g.Id)))
            .WhereIf(request.Platforms is { Length: > 0 }, x => x.Platforms.Any(p => request.Platforms.Contains(p.Id)))
            .WhereIf(request.Publisher.HasValue, x => x.PublisherId == request.Publisher)
            .WhereIf(request.IsAvailable is true, x => x.AvailableStock > 0);
}