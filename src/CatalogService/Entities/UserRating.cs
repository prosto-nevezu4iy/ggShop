namespace CatalogService.Entities;

public class UserRating
{
    public Guid Id { get; set; }
    public Guid GameId { get; set; }
    public Game Game { get; set; }
    public Guid UserId { get; set; }
    public byte Rating { get; set; }
}