using System.Security.Claims;
using Duende.IdentityModel;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityService.Pages.Account.Manage.Profile;

public class Index(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) : PageModel
{
    [BindProperty] public ProfileVIewModel Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
        }

        LoadAsync(user);
        return Page();
    }

    public  async Task<IActionResult> OnPostAsync()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
        }

        if (!ModelState.IsValid)
        {
            LoadAsync(user);
            return Page();
        }

        user.UserName = Input.Username;
        user.FirstName = Input.FirstName;
        user.LastName = Input.LastName;
        user.DateOfBirth = Input.DateOfBirth;
        user.AvatarUrl = Input.AvatarUrl;

        await userManager.UpdateAsync(user);

        var existingClaims = await userManager.GetClaimsAsync(user);

        await ReplaceClaimAsync(user, existingClaims, JwtClaimTypes.NickName, user.UserName ?? "");
        await ReplaceClaimAsync(user, existingClaims, JwtClaimTypes.GivenName, user.FirstName ?? "");
        await ReplaceClaimAsync(user, existingClaims, JwtClaimTypes.FamilyName, user.LastName ?? "");

        await signInManager.RefreshSignInAsync(user);

        Input.StatusMessage = "Your profile has been updated";

        return Page();
    }

    private void LoadAsync(ApplicationUser user)
    {
        Input = new ProfileVIewModel
        {
            Username = user.UserName,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            DateOfBirth = user.DateOfBirth,
            AvatarUrl = user.AvatarUrl
        };
    }

    private async Task ReplaceClaimAsync(ApplicationUser user, IEnumerable<Claim> existingClaims, string type, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return; // Skip empty values — don't add or remove anything

        var oldClaim = existingClaims.FirstOrDefault(c => c.Type == type);
        if (oldClaim != null)
            await userManager.ReplaceClaimAsync(user, oldClaim, new Claim(type, value));
        else
            await userManager.AddClaimAsync(user, new Claim(type, value));
    }
}