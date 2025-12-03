namespace CatalogService.DTOs;

public record UpdateGameDto(
    string Description,
    string FullDescription,
    byte Discount,
    uint AvailableStock,
    List<Guid> Platforms,
    byte Rating,
    bool IsPreOrder,
    IReadOnlyCollection<string> ScreenShotUrls
)
{
    public List<Guid> Platforms { get; init; } = Platforms ?? [];
    public IReadOnlyCollection<string> ScreenShotUrls { get; init; } = ScreenShotUrls ?? [];
}
