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

namespace IdentityService.Pages.Account.ForgotPassword;

[AllowAnonymous]
public class Index(UserManager<ApplicationUser> userManager, IEmailSender emailSender) : PageModel
{
    [BindProperty]
    public required ForgotPasswordViewModel Input { get; set; }

    public void OnGet()
    {

    }

    public async Task<IActionResult> OnPost()
    {
        if (ModelState.IsValid)
        {
            var user = await userManager.FindByEmailAsync(Input.Email);
            if (user == null || !(await userManager.IsEmailConfirmedAsync(user)))
            {
                // Don't reveal that the user does not exist or is not confirmed
                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            // For more information on how to enable account confirmation and password reset please
            // visit https://go.microsoft.com/fwlink/?LinkID=532713
            var code = await userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ResetPassword/Index",
                pageHandler: null,
                values: new { code },
                protocol: Request.Scheme)!;

            var emailParams = new Dictionary<string, string>
            {
                { Email.Username, user.UserName! },
                { Email.ResetLink, HtmlEncoder.Default.Encode(callbackUrl) }
            };

            await emailSender.SendEmailAsync(Input.Email, emailParams, Email.ForgotPasswordTemplateId);

            return RedirectToPage("/Account/ForgotPasswordConfirmation/Index");
        }

        return Page();
    }
}