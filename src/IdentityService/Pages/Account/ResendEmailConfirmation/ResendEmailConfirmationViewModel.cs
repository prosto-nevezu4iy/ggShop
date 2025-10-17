using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Pages.Account.ResendEmailConfirmation;

public class ResendEmailConfirmationViewModel
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [TempData] public string? StatusMessage { get; set; } = string.Empty;
}