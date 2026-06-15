using Common.Infrastructure.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

namespace Common.Presentation.Extensions;

public static class OpenApiExtensions
{
    public static void AddCommonSwagger(this IServiceCollection services, IdentitySettings identitySettings)
    {
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition(nameof(SecuritySchemeType.OAuth2), new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Description = "OAuth2 Authorization Code with PKCE",
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri(identitySettings.AuthorizationUrl),
                        TokenUrl = new Uri(identitySettings.TokenUrl),
                        Scopes = identitySettings.Scopes
                    }
                }
            });

            options.AddSecurityRequirement(document => new()
            {
                [new OpenApiSecuritySchemeReference(nameof(SecuritySchemeType.OAuth2), document)] = []
            });
        });
    }

    public static void UseCommonScalarUi(this WebApplication app, IdentitySettings identitySettings, string apiTitle)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapSwagger("/openapi/{documentName}.json");
            app.MapScalarApiReference(options =>
            {
                options.WithTitle(apiTitle);
                options.AddPreferredSecuritySchemes(nameof(SecuritySchemeType.OAuth2));
                options.AddAuthorizationCodeFlow(nameof(SecuritySchemeType.OAuth2), flow =>
                {
                    flow.ClientId = identitySettings.ClientId;
                    flow.Pkce = Pkce.Sha256;
                    flow.SelectedScopes = identitySettings.Scopes.Keys;
                });
            });
        }
    }
}