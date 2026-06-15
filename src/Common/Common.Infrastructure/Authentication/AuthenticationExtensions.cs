using Common.Infrastructure.Authorization;
using Common.Infrastructure.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Infrastructure.Authentication;

public static class AuthenticationExtensions
{
    public static void AddCommonAuthentication(this IServiceCollection services, IdentitySettings identitySettings)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = identitySettings.Authority;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters.ValidateAudience = false;
                options.TokenValidationParameters.NameClaimType = IdentityConstants.IdentityUserName;
            });
    }
}