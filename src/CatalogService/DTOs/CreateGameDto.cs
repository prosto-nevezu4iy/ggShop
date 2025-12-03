namespace CatalogService.DTOs;

public record CreateGameDto(
    string Name,
    string Description,
    string FullDescription,
    string SystemRequirements,
    decimal Price,
    byte Discount,
    uint AvailableStock,
    Guid Publisher,
    DateOnly ReleaseDate,
    byte Rating,
    bool IsPreOrder,
    string ImageUrl,
    string TrailerUrl,
    string BackgroundUrl,
    IReadOnlyCollection<string> ScreenShotUrls,
    IReadOnlyCollection<Guid> Genres,
    IReadOnlyCollection<Guid> Platforms
)
{
    public IReadOnlyCollection<Guid> Platforms { get; init; } = Platforms ?? [];
    public IReadOnlyCollection<string> ScreenShotUrls { get; init; } = ScreenShotUrls ?? [];
    public IReadOnlyCollection<Guid> Genres { get; init; } = Genres ?? [];
}
