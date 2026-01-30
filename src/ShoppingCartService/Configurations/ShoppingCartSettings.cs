namespace ShoppingCartService.Configurations;

public sealed class ShoppingCartSettings
{
    public const string Section = "ShoppingCart";

    public int BatchSize { get; set; } = 128;
    public int ExpirationDays { get; set; } = 30;
    public int MaxItemsPerCart { get; set; } = 2;
}