using System.Text;
using IdentityService.Constants;
using IdentityService.Models;
using IdentityService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using static IdentityService.Constants.ErrorMessages;
using static IdentityService.Constants.SuccessMessages;
using IdentityConstants = IdentityService.Constants.IdentityConstants;

namespace IdentityService.Pages.Account.ResendEmailConfirmation;

[AllowAnonymous]
public class Index(UserManager<ApplicationUser> userManager, IEmailSender emailSender) : PageModel
{
    [BindProperty]
    public ResendEmailConfirmationViewModel Input { get; set; } = new();

    public void OnGet()
    {

    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await userManager.FindByEmailAsync(Input.Email);
        if (user is null)
        {
            return NotFound(string.Format(UserNotFoundWithEmail, Input.Email));
        }

        var emailConfirmationUrl = await GetEmailConfirmationUrl(user);

        var emailParams = new Dictionary<string, string>
        {
            { IdentityConstants.Username, user.UserName },
            { IdentityConstants.Link, emailConfirmationUrl ?? string.Empty }
        };

        await emailSender.SendEmailAsync(user.Email, emailParams, IdentityConstants.AccountConfirmationTemplateId);

        Input.StatusMessage = VerificationEmailSent;

        return Page();
    }

    private async Task<string> GetEmailConfirmationUrl(ApplicationUser user)
    {
        var userId = await userManager.GetUserIdAsync(user);

        var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        return Url.Page(
            PageRoutes.ConfirmEmail,
            pageHandler: null,
            values: new { userId, code },
            protocol: Request.Scheme);
    }
}