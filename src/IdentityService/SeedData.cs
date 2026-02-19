using System.Security.Claims;
using Common.Infrastructure.Authentication;
using Common.Infrastructure.Authorization;
using Duende.IdentityModel;
using IdentityService.Constants;
using IdentityService.Data;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace IdentityService;

public static class SeedData
{
    public static async Task EnsureSeedData(WebApplication app)
    {
        using var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();

        await MigrateDatabaseAsync(scope);
        await SeedUsersAsync(scope);
        await SeedRolesAsync(scope);
        await SeedPermissionsAsync(scope);
        await SeedRolesPermissionsAsync(scope);
    }
    private static async Task MigrateDatabaseAsync(IServiceScope scope)
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync();
    }

    private static async Task SeedRolesAsync(IServiceScope scope)
    {
        var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        if (roleMgr.Roles.Any()) return;

        foreach (var role in Enum.GetNames<Roles>())
        {
            if (!await roleMgr.RoleExistsAsync(role))
            {
                await roleMgr.CreateAsync(new IdentityRole(role));
            }
        }
    }

    private static async Task SeedPermissionsAsync(IServiceScope scope)
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (context.Permissions.Any()) return;

        foreach (var permissionName in PermissionsList.All)
        {
            var exists = await context.Permissions
                .AnyAsync(p => p.Name == permissionName);

            if (!exists)
            {
                context.Permissions.Add(new Permission
                {
                    Name = permissionName
                });
            }
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedRolesPermissionsAsync(IServiceScope scope)
    {
        var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (context.RolePermissions.Any()) return;

        var adminRole = await roleMgr.FindByNameAsync(nameof(Roles.Admin));
        var userRole = await roleMgr.FindByNameAsync(nameof(Roles.User));

        var allPermissions = await context.Permissions.ToListAsync();

        foreach (var permission in allPermissions)
        {
            if (!context.RolePermissions
                    .Any(rp => rp.RoleId == adminRole.Id &&
                               rp.PermissionId == permission.Id))
            {
                context.RolePermissions.Add(new RolePermission
                {
                    RoleId = adminRole?.Id,
                    PermissionId = permission.Id
                });
            }
        }

        var userPermissions = allPermissions
            .Where(p => PermissionsList.UserPermissions.Contains(p.Name));

        foreach (var permission in userPermissions)
        {
            if (!context.RolePermissions
                    .Any(rp => rp.RoleId == userRole.Id &&
                               rp.PermissionId == permission.Id))
            {
                context.RolePermissions.Add(new RolePermission
                {
                    RoleId = userRole?.Id,
                    PermissionId = permission.Id
                });
            }
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedUsersAsync(IServiceScope scope)
    {
        var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        if (userMgr.Users.Any()) return;

        await CreateUserAsync(userMgr, new ApplicationUser
        {
            UserName = "alice",
            Email = "AliceSmith@example.com",
            FirstName = "Alice",
            LastName = "Smith",
            DateOfBirth = new DateOnly(1967, 5, 21)
        });

        await CreateUserAsync(userMgr, new ApplicationUser
        {
            UserName = "bob",
            Email = "BobSmith@example.com",
            FirstName = "Bob",
            LastName = "Smith",
            DateOfBirth = new DateOnly(1987, 7, 12)
        });
    }

    private static async Task CreateUserAsync(UserManager<ApplicationUser> userMgr, ApplicationUser userData)
    {
        var user = await userMgr.FindByNameAsync(userData.UserName!);
        if (user is not null)
        {
            Log.Debug("{UserName} already exists", userData.UserName);
            return;
        }

        user = new ApplicationUser
        {
            UserName = userData.UserName,
            Email = userData.Email,
            EmailConfirmed = true,
            FirstName = userData.FirstName,
            LastName = userData.LastName,
            DateOfBirth = userData.DateOfBirth
        };

        var result = await userMgr.CreateAsync(user, "Pass123$");
        if (!result.Succeeded)
        {
            throw new Exception(result.Errors.First().Description);
        }

        var claims = new[]
        {
            new Claim(JwtClaimTypes.NickName, userData.UserName),
            new Claim(JwtClaimTypes.GivenName, userData.FirstName),
            new Claim(JwtClaimTypes.FamilyName, userData.LastName)
        };

        result = await userMgr.AddClaimsAsync(user, claims);
        if (!result.Succeeded)
        {
            throw new Exception(result.Errors.First().Description);
        }

        Log.Debug("{UserName} created", userData.UserName);
    }
}
