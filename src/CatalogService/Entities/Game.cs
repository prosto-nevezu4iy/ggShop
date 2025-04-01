using NpgsqlTypes;

namespace CatalogService.Entities;

public class Game
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string FullDescription { get; set; }
    public string SystemRequirements { get; set; }
    public decimal Price { get; set; }
    public int Discount { get; set; }
    public decimal DiscountPrice { get; set; }
    public int AvailableStock { get; set; }
    public List<Platform> Platforms { get; set; } = new();
    public Guid PublisherId { get; set; }
    public Publisher Publisher { get; set; }
    public DateOnly ReleaseDate { get; set; }
    public int Rating { get; set; }
    public bool IsPreOrder { get; set; }
    public string ImageUrl { get; set; }
    public string TrailerUrl { get; set; }
    public string BackgroundUrl { get; set; }
    public ICollection<string> ScreenShotUrls { get; set; } = new List<string>();
    public List<Genre> Genres { get; set; } = new();
    public NpgsqlTsVector SearchVector { get; set; }
}
