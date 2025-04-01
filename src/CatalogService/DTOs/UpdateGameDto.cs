namespace CatalogService.DTOs;

public record UpdateGameDto
{
    public string Description { get; init; }
    public string FullDescription { get; init; }
    public int Discount { get; init; }
    public int AvailableStock { get; init; }
    public List<Guid> Platforms { get; init; } = new();
    public int Rating { get; init; }
    public bool IsPreOrder { get; init; }
    public ICollection<string> ScreenShotUrls { get; init; } = new List<string>();
}
