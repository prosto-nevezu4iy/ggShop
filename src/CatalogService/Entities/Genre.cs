namespace CatalogService.Entities;

public class Genre
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<Game> Games { get; set; } = new();
}
