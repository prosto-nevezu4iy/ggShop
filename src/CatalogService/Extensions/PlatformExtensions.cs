using CatalogService.DTOs.Platforms;
using CatalogService.Entities;

namespace CatalogService.Extensions;

public static class PlatformExtensions
{
    public static PlatformDto ToDto(this Platform platform)
    {
        return new PlatformDto(platform.Id, platform.Name);
    }

    public static Platform ToEntity(this CreatePlatformDto createPlatformDto)
    {
        return new Platform
        {
            Name = createPlatformDto.Name
        };
    }
}