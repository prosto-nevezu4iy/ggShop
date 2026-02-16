using System.Text;
using System.Text.Encodings.Web;
using IdentityService.Constants;
using IdentityService.Models;
using IdentityService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using IdentityConstants = IdentityService.Constants.IdentityConstants;

namespace IdentityService.Pages.Account.ForgotPassword;

[AllowAnonymous]
public class Index(UserManager<ApplicationUser> userManager, IEmailSender emailSender) : PageModel
{
    [BindProperty]
    public ForgotPasswordViewModel Input { get; set; }

    public void OnGet()
    {

    }

    public async Task<IActionResult> OnPost()
    {
        if (ModelState.IsValid)
        {
            var user = await userManager.FindByEmailAsync(Input.Email);
            if (user is null || !await userManager.IsEmailConfirmedAsync(user))
            {
                // Don't reveal that the user does not exist or is not confirmed
                return RedirectToPage(PageRoutes.ForgotPasswordConfirmation);
            }

            var code = await userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var callbackUrl = Url.Page(
                PageRoutes.ResetPassword,
                pageHandler: null,
                values: new { code },
                protocol: Request.Scheme)!;

            var emailParams = new Dictionary<string, string>
            {
                { IdentityConstants.Username, user.UserName },
                { IdentityConstants.ResetLink, HtmlEncoder.Default.Encode(callbackUrl) }
            };

            await emailSender.SendEmailAsync(Input.Email, emailParams, IdentityConstants.ForgotPasswordTemplateId);

            return RedirectToPage(PageRoutes.ForgotPasswordConfirmation);
        }

        return Page();
    }
}