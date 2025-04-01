namespace CatalogService.DTOs;

public record PlatformDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
}
