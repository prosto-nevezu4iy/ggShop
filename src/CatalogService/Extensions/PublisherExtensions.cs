using CatalogService.DTOs.Publishers;
using CatalogService.Entities;

namespace CatalogService.Extensions;

public static class PublisherExtensions
{
    public static PublisherDto ToDto(this Publisher platform)
    {
        return new PublisherDto(platform.Id, platform.Name);
    }

    public static Publisher ToEntity(this CreatePublisherDto createPublisherDto)
    {
        return new Publisher
        {
            Name = createPublisherDto.Name
        };
    }
}