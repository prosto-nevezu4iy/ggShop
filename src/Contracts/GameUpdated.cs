namespace Contracts;

public class GameUpdated
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public string FullDescription { get; set; }
    public int Discount { get; set; }
    public int AvailableStock { get; set; }
    public List<int> Platforms { get; init; } = new();
    public ICollection<string> ScreenShotUrls { get; init; } = new List<string>();
}