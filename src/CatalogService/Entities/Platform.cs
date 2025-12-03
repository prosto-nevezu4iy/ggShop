namespace CatalogService.Entities;

public class Platform
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public ICollection<Game> Games { get; set; } = [];
}
