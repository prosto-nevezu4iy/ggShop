using Microsoft.AspNetCore.Authorization;

namespace Common.Infrastructure.Authorization;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class RequirePermissionAttribute(params string[] permissions)
    : AuthorizeAttribute(policy: string.Join(",", permissions))
{
}