namespace CatalogService.DTOs;

public record GameDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public string FullDescription { get; init; }
    public string SystemRequirements { get; init; }
    public decimal Price { get; init; }
    public int Discount { get; init; }
    public decimal DiscountPrice { get; init; }
    public int AvailableStock { get; init; }
    public List<PlatformDto> Platforms { get; init; } = new();
    public PublisherDto Publisher { get; init; }
    public DateOnly ReleaseDate { get; init; }
    public int Rating { get; init; }
    public bool IsPreOrder { get; init; }
    public string ImageUrl { get; init; }
    public string TrailerUrl { get; init; }
    public string BackgroundUrl { get; init; }
    public ICollection<string> ScreenShotUrls { get; init; } = new List<string>();
    public List<GenreDto> Genres { get; init; } = new();
}
