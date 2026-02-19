using System.Text;
using IdentityService.Abstractions;
using IdentityService.Constants;
using IdentityService.Models;
using IdentityService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using static IdentityService.Constants.ErrorMessages;
using IdentityConstants = IdentityService.Constants.IdentityConstants;

namespace IdentityService.Pages.Account.RegisterConfirmation;

[AllowAnonymous]
public class Index(UserManager<ApplicationUser> userManager, IEmailSender emailSender) : PageModel
{
    public async Task<IActionResult> OnGet(string email, string returnUrl = null)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return RedirectToPage("/Index");
        }
        returnUrl ??= Url.Content("~/");

        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return NotFound(string.Format(UserNotFoundWithEmail, email));
        }

        var emailConfirmationUrl = await GetEmailConfirmationUrl(returnUrl, user);

        var emailParams = new Dictionary<string, string>
        {
            { IdentityConstants.Username, user.UserName },
            { IdentityConstants.Link, emailConfirmationUrl ?? string.Empty }
        };

        await emailSender.SendEmailAsync(user.Email, emailParams, IdentityConstants.AccountConfirmationTemplateId);

        return Page();
    }

    private async Task<string?> GetEmailConfirmationUrl(string returnUrl, ApplicationUser user)
    {
        var userId = await userManager.GetUserIdAsync(user);

        var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        return Url.Page(
            PageRoutes.ConfirmEmail,
            pageHandler: null,
            values: new { userId, code, returnurl = returnUrl },
            protocol: Request.Scheme);
    }
}