using Microsoft.AspNetCore.Identity;

namespace IdentityService.Models;

public class RolePermission
{
    public string RoleId { get; set; }
    public IdentityRole Role { get; set; }

    public Guid PermissionId { get; set; }
    public Permission Permission { get; set; }
}