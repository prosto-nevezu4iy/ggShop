using System.Security.Claims;
using System.Text;
using Duende.IdentityModel;
using IdentityService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace IdentityService.Pages.Account.Register;

[SecurityHeaders]
[AllowAnonymous]
public class Index(UserManager<ApplicationUser> userManager) : PageModel
{
    [BindProperty] public required RegisterViewModel Input { get; set; }
    [BindProperty] public bool RegisterSuccess { get; set; }

    public IActionResult OnGet(string? returnUrl = null)
    {
        Input.ReturnUrl = returnUrl;
        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        if (Input.Button != "register") return Redirect("~/");

        if (ModelState.IsValid)
        {
            var user = new ApplicationUser
            {
                UserName = Input.Email.Split("@")[0],
                Email = Input.Email
            };

            var result = await userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                await userManager.AddClaimsAsync(user, [
                    new Claim(JwtClaimTypes.NickName, user.UserName),
                ]);

                RegisterSuccess = true;

                return RedirectToPage("/Account/RegisterConfirmation/Index", new { email = Input.Email, returnUrl = Input.ReturnUrl });
            }
        }

        return Page();
    }
}