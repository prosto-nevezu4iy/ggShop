namespace CatalogService.Configurations;

public record CloudinarySettings(string CloudName, string ApiKey, string ApiSecret)
{
    public CloudinarySettings() : this(string.Empty, string.Empty, string.Empty)
    {
    }
}