namespace CatalogService.DTOs;

public record GenreDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
}