using System.ComponentModel.DataAnnotations;

namespace IdentityService.Pages.Account.ForgotPassword;

public class ForgotPasswordViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}