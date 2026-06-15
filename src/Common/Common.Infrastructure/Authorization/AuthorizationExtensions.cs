using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Common.Infrastructure.Authorization;

public static class AuthorizationExtensions
{
    public static void AddCommonAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization();
    }
    
    public static IdentitySettings AddCommonIdentitySettings(this IHostApplicationBuilder builder)
    {
        builder.Services
            .AddOptions<IdentitySettings>()
            .BindConfiguration(IdentitySettings.Section)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return builder.Configuration
                   .GetSection(IdentitySettings.Section)
                   .Get<IdentitySettings>() 
               ?? throw new InvalidOperationException($"'{IdentitySettings.Section}' section is missing from configuration.");
    }
}