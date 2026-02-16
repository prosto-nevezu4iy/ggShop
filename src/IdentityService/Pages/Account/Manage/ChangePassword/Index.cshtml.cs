using IdentityService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static IdentityService.Constants.ErrorMessages;
using static IdentityService.Constants.SuccessMessages;

namespace IdentityService.Pages.Account.Manage.ChangePassword;

public class Index(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<ChangePasswordViewModel> logger) : PageModel
{
    [BindProperty]
    public ChangePasswordViewModel Input { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return NotFound(string.Format(UserNotFoundWithId, userManager.GetUserId(User)));
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return NotFound(string.Format(UserNotFoundWithId, userManager.GetUserId(User)));
        }

        var changePasswordResult = await userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
        if (!changePasswordResult.Succeeded)
        {
            foreach (var error in changePasswordResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return Page();
        }

        await signInManager.RefreshSignInAsync(user);
        logger.LogInformation(LoggerEventIds.PasswordChanged, "User changed their password successfully.");
        Input.StatusMessage = PasswordHasBeenChanged;

        return Page();
    }
}