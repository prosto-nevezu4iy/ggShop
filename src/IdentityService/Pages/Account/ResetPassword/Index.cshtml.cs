using System.Text;
using IdentityService.Constants;
using IdentityService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using static IdentityService.Constants.ErrorMessages;

namespace IdentityService.Pages.Account.ResetPassword;

[AllowAnonymous]
public class Index(UserManager<ApplicationUser> userManager) : PageModel
{
    [BindProperty]
    public ResetPasswordViewModel Input { get; set; }

    public IActionResult OnGet(string code = null)
    {
        if (code is null)
        {
            return BadRequest(MustProvideCode);
        }

        Input = new ResetPasswordViewModel
        {
            Email = string.Empty,
            Password = string.Empty,
            Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await userManager.FindByEmailAsync(Input.Email);
        if (user == null)
        {
            // Don't reveal that the user does not exist
            return RedirectToPage(PageRoutes.ResetPasswordConfirmation);
        }

        var result = await userManager.ResetPasswordAsync(user, Input.Code, Input.Password);
        if (result.Succeeded)
        {
            return RedirectToPage(PageRoutes.ResetPasswordConfirmation);
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return Page();
    }
}