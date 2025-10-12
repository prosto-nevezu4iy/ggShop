using System.Text;
using System.Text.Encodings.Web;
using IdentityService.Models;
using IdentityService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace IdentityService.Pages.Account.ResendEmailConfirmation;

[AllowAnonymous]
public class Index(UserManager<ApplicationUser> userManager, IEmailSender emailSender) : PageModel
{
    [BindProperty] public ResendEmailConfirmationViewModel Input { get; set; } = new();

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
        if (user == null)
        {
            return NotFound($"Unable to load user with email '{Input.Email}'.");
        }

        var emailConfirmationUrl = await GetEmailConfirmationUrl(user);

        var emailParams = new Dictionary<string, string>
        {
            { "USERNAME", user.UserName },
            { "LINK", emailConfirmationUrl ?? string.Empty }
        };

        await emailSender.SendEmailAsync(user.Email, emailParams, 1);

        Input.StatusMessage = "Verification email sent. Please check your email.";

        return Page();
    }

    private async Task<string?> GetEmailConfirmationUrl(ApplicationUser user)
    {
        var userId = await userManager.GetUserIdAsync(user);

        var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        return Url.Page(
            "/Account/ConfirmEmail/Index",
            pageHandler: null,
            values: new { userId = userId, code = code },
            protocol: Request.Scheme);
    }
}