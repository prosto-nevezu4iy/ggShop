using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Pages.Account.ResendEmailConfirmation;

public class ResendEmailConfirmationViewModel
{
    [Required]
    [EmailAddress]
    public string? Email { get; set; } = string.Empty;

    [TempData] public string? StatusMessage { get; set; } = string.Empty;
}