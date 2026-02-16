using System.Text;
using IdentityService.Constants;
using IdentityService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using static IdentityService.Constants.IdentityConstants;
using static IdentityService.Constants.ErrorMessages;

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
            return RedirectToPage(PageRoutes.Home);
        }

        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return NotFound(string.Format(UserNotFoundWithId, userId));
        }

        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

        var result = await userManager.ConfirmEmailAsync(user, code);
        Input.StatusMessage = result.Succeeded ? ConfirmEmailSuccess : ConfirmEmailFailure;

        return Page();
    }
}