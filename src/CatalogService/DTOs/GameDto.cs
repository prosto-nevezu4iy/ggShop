namespace CatalogService.DTOs;

public record GameDto(
    Guid Id,
    string Name,
    string Description,
    string FullDescription,
    string SystemRequirements,
    decimal Price,
    byte Discount,
    decimal DiscountPrice,
    uint AvailableStock,
    PublisherDto Publisher,
    DateOnly ReleaseDate,
    byte Rating,
    double? AverageUserRating,
    uint TotalUserRatings,
    bool IsPreOrder,
    string ImageUrl,
    string TrailerUrl,
    string BackgroundUrl,
    IReadOnlyCollection<string> ScreenShotUrls,
    IReadOnlyCollection<GenreDto> Genres,
    IReadOnlyCollection<PlatformDto> Platforms)
{
    public IReadOnlyCollection<PlatformDto> Platforms { get; init; } = Platforms ?? [];
    public IReadOnlyCollection<string> ScreenShotUrls { get; init; } = ScreenShotUrls ?? [];
    public IReadOnlyCollection<GenreDto> Genres { get; init; } = Genres ?? [];
}
