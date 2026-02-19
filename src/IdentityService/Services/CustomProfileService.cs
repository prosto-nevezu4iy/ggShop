using System.Globalization;
using System.Security.Claims;
using Common.Infrastructure.Authentication;
using Common.Infrastructure.Authorization;
using Duende.IdentityModel;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityService.Abstractions;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;
using static Common.Infrastructure.Authentication.CustomClaims;

namespace IdentityService.Services;

public class CustomProfileService : IProfileService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IPermissionService _permissionService;

    public CustomProfileService(UserManager<ApplicationUser> userManager, IPermissionService permissionService, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _permissionService = permissionService;
        _roleManager = roleManager;
    }

    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var user = await _userManager.GetUserAsync(context.Subject)
            ?? throw new Exception("User not found");

        var claims = new List<Claim>();

        AddUserClaims(user, claims);
        await AddRolesClaimsAsync(user, claims);
        await AddExistingNameClaimAsync(user, claims);

        context.IssuedClaims.AddRange(claims);
    }

    public Task IsActiveAsync(IsActiveContext context)
    {
        return Task.CompletedTask;
    }

    private static void AddUserClaims(ApplicationUser user, List<Claim> claims)
    {
        AddClaimIfNotEmpty(claims, ClaimTypes.Name, user.UserName);
        AddClaimIfNotEmpty(claims, ClaimTypes.GivenName, user.FirstName);
        AddClaimIfNotEmpty(claims, ClaimTypes.Surname, user.LastName);
        AddClaimIfNotEmpty(claims, AvatarUrl, user.AvatarUrl);

        if (user.DateOfBirth.HasValue)
        {
            claims.Add(new Claim(
                ClaimTypes.DateOfBirth,
                user.DateOfBirth.Value.ToString(CultureInfo.InvariantCulture),
                ClaimValueTypes.DateTime
            ));
        }
    }

    private async Task AddRolesClaimsAsync(ApplicationUser user, List<Claim> claims)
    {
        var roles = await _userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(JwtClaimTypes.Role, role)));

        await AddPermissionsClaimsAsync(user.Id, roles, claims);
    }

    private async Task AddPermissionsClaimsAsync(string userId, IEnumerable<string> roles, List<Claim> claims)
    {
        var permissions = await _permissionService.GetPermissionsByUserIdAsync(userId, roles);
        claims.AddRange(permissions.Select(permission => new Claim(CustomClaims.Permission, permission)));
    }

    private async Task AddExistingNameClaimAsync(ApplicationUser user, List<Claim> claims)
    {
        var existingClaims = await _userManager.GetClaimsAsync(user);
        var nameClaim = existingClaims.FirstOrDefault(x => x.Type == JwtClaimTypes.Name);

        if (nameClaim is not null)
        {
            claims.Add(nameClaim);
        }
    }

    private static void AddClaimIfNotEmpty(List<Claim> claims, string claimType, string? value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            claims.Add(new Claim(claimType, value));
        }
    }
}