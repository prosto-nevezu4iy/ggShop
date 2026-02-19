namespace IdentityService.Abstractions;

public interface IPermissionService
{
    Task<IEnumerable<string>> GetPermissionsByUserIdAsync(string userId, IEnumerable<string> roles);
}