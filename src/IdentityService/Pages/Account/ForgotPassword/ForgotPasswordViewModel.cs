using System.ComponentModel.DataAnnotations;

namespace IdentityService.Pages.Account.ForgotPassword;

public class ForgotPasswordViewModel
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }
}