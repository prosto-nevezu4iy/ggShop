using System.ComponentModel.DataAnnotations;

namespace IdentityService.Pages.Account.Register;

public class RegisterViewModel
{
    [Required]
    public required string Email { get; set; }

    [Required]
    public required string Password { get; set; }

    public string? ReturnUrl { get; set; } = string.Empty;
    public string? Button { get; set; }
}