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
    public byte Discount { get; set; }
    public decimal DiscountPrice { get; set; }
    public uint AvailableStock { get; set; }
    public Guid PublisherId { get; set; }
    public Publisher Publisher { get; set; }
    public DateOnly ReleaseDate { get; set; }
    public byte Rating { get; set; }
    public double? AverageUserRating => UserRatings.Any() ? UserRatings.Average(ur => ur.Rating) : null;
    public uint TotalUserRatings => (uint)UserRatings.Count;
    public bool IsPreOrder { get; set; }
    public string ImageUrl { get; set; }
    public string TrailerUrl { get; set; }
    public string BackgroundUrl { get; set; }
    public ICollection<string> ScreenShotUrls { get; set; } = [];
    public ICollection<Genre> Genres { get; set; } = [];
    public List<Platform> Platforms { get; set; } = [];
    public ICollection<UserRating> UserRatings { get; set; } = [];
    public NpgsqlTsVector SearchVector { get; set; }
}
