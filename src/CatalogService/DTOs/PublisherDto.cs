namespace CatalogService.DTOs;

public record PublisherDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
}
