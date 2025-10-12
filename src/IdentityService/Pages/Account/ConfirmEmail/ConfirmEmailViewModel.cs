using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Pages.Account.ConfirmEmail;

public class ConfirmEmailViewModel
{
    [TempData] public string? StatusMessage { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; } = string.Empty;
}