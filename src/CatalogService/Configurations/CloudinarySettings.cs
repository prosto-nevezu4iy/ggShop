using System.ComponentModel.DataAnnotations;

namespace CatalogService.Configurations;

public sealed class CloudinarySettings
{
    [Required]
    public string CloudName { get; init; }

    [Required]
    public string ApiKey { get; init; }

    [Required]
    public string ApiSecret { get; init; }
}