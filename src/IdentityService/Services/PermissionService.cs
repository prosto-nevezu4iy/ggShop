using IdentityService.Abstractions;
using IdentityService.Data;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace IdentityService.Services;

public class PermissionService : IPermissionService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMemoryCache _cache;

    public PermissionService(ApplicationDbContext dbContext, IMemoryCache cache, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
    {
        _dbContext = dbContext;
        _cache = cache;
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task<IEnumerable<string>> GetPermissionsByUserIdAsync(string userId, IEnumerable<string> roles)
    {
        var cacheKey = $"user_permissions_{userId}";

        if (_cache.TryGetValue(cacheKey, out IEnumerable<string> cachedPermissions))
        {
            return cachedPermissions;
        }

        var permissionTasks = roles.Select(async role =>
        {
            var roleEntity = await _roleManager.FindByNameAsync(role);
            return await _dbContext.RolePermissions
                .Where(rp => rp.RoleId == roleEntity.Id)
                .Select(rp => rp.Permission.Name)
                .ToListAsync();
        });

        var permissions = (await Task.WhenAll(permissionTasks))
            .SelectMany(p => p)
            .Distinct()
            .ToList();

        _cache.Set(cacheKey, permissions, TimeSpan.FromMinutes(30));

        return permissions;
    }
}