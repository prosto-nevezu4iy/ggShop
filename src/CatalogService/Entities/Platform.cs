namespace CatalogService.Entities;

public class Platform
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<Game> Games { get; set; } = new();
}
