using System.ComponentModel.DataAnnotations;

namespace Common.Application.Configurations;

public sealed class IdentitySettings
{
    public const string Section = "Identity";

    [Required]
    public string Authority { get; init; }

    [Required]
    public string AuthorizationUrl { get; init; }

    [Required]
    public string TokenUrl { get; init; }

    [Required]
    public string ClientId { get; init; }

    [Required]
    public Dictionary<string, string> Scopes { get; set; } = new();
}