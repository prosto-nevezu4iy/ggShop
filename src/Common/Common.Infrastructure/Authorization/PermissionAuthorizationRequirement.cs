using Common.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace Common.Infrastructure.Authorization;

public class PermissionAuthorizationRequirement(params string[] allowedPermissions)
    : AuthorizationHandler<PermissionAuthorizationRequirement>, IAuthorizationRequirement
{
    private string[] AllowedPermissions { get; } = allowedPermissions;

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionAuthorizationRequirement requirement)
    {
        foreach (var permission in requirement.AllowedPermissions)
        {
            var found = context.User.FindFirst(c =>
                c.Type == CustomClaims.Permission &&
                c.Value == permission) is not null;

            if (found)
            {
                context.Succeed(requirement);
                break;
            }
        }
        return Task.CompletedTask;
    }
}