using System.Text;
using IdentityService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace IdentityService.Pages.Account.ConfirmEmail;

[AllowAnonymous]
public class Index(UserManager<ApplicationUser> userManager) : PageModel
{
    [BindProperty] public ConfirmEmailViewModel Input { get; set; } = new();

    public async Task<IActionResult> OnGet(string userId, string code, string returnUrl)
    {
        Input.ReturnUrl = returnUrl;

        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
        {
            return RedirectToPage("/Index");
        }

        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return NotFound($"Unable to load user with ID '{userId}'.");
        }

        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

        var result = await userManager.ConfirmEmailAsync(user, code);
        Input.StatusMessage = result.Succeeded ? "Thank you for confirming your email." : "Error confirming your email.";

        return Page();
    }
}