using System.Text.Json.Serialization;
using Common.Application.Configurations;
using Common.Presentation.Middlewares;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Serilog;
using ShoppingCartService.Abstractions;
using ShoppingCartService.Configurations;
using ShoppingCartService.Consumers;
using ShoppingCartService.Repositories;
using ShoppingCartService.Services;
using StackExchange.Redis;
using static Common.Application.Constants.DatabaseConstants;
using static Common.Application.Constants.IdentityConstants;
using static ShoppingCartService.Constants.ShoppingCartConstants;

namespace ShoppingCartService.Extensions;

public static class ServiceExtensions
{
    public static void AddHostBuilderServices(this IHostBuilder hostBuilder)
    {
        hostBuilder.UseSerilog((context, loggerConfiguration) =>
            loggerConfiguration.ReadFrom.Configuration(context.Configuration));
    }

    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
        
        builder.Services
            .AddOptions<ShoppingCartSettings>()
            .BindConfiguration(ShoppingCartSettings.Section);

        builder.Services.AddGrpc();

        builder.Services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString(DefaultConnection) ?? throw new InvalidOperationException()));

        AddShoppingCartServices(builder);

        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();

        builder.Services.AddMassTransit(x =>
        {
            x.AddConsumersFromNamespaceContaining<GameDeletedConsumer>();

            x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter(EndpointName, false));

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.ReceiveEndpoint(GameDeletedEndpoint, e =>
                {
                    e.UseMessageRetry(r => r.Interval(5, 5));

                    e.ConfigureConsumer<GameDeletedConsumer>(context);
                });
                cfg.ReceiveEndpoint(UserLoggedInEndpoint, e =>
                {
                    e.UseMessageRetry(r => r.Interval(5, 5));

                    e.ConfigureConsumer<UserLoggedInConsumer>(context);
                });
                cfg.ConfigureEndpoints(context);
            });
        });
        
        var identitySettings = builder.Configuration.GetSection(IdentitySettings.Section).Get<IdentitySettings>();

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = builder.Configuration.GetValue<string>(identitySettings.Authority);
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters.ValidateAudience = false;
                options.TokenValidationParameters.NameClaimType = IdentityUserName;
            });

        builder.Services.AddAuthorization();
        
        builder.Services.AddSwaggerGen(options =>
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
    
    public static void AddMiddlewares(this WebApplication app, IConfiguration configuration)
    {
        var identitySettings = configuration.GetSection(IdentitySettings.Section).Get<IdentitySettings>();
        if (app.Environment.IsDevelopment())
        {
            app.MapSwagger("/openapi/{documentName}.json");
            app.MapScalarApiReference(options =>
            {
                options.WithTitle("ShoppingCart Service API");
                options.AddPreferredSecuritySchemes(nameof(SecuritySchemeType.OAuth2));
                options.AddAuthorizationCodeFlow(nameof(SecuritySchemeType.OAuth2), flow =>
                {
                    flow.ClientId = identitySettings.ClientId;
                    flow.Pkce = Pkce.Sha256;
                    flow.SelectedScopes = identitySettings.Scopes.Keys;
                });
            });
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseSerilogRequestLogging();

        app.UseExceptionHandler();

        app.MapControllers();

        app.MapGrpcService<GrpcShoppingCartService>();
    }

    private static void AddShoppingCartServices(IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IShoppingCartRepository, RedisShoppingCartRepository>();

        builder.Services.AddScoped<IShoppingCartService, Services.ShoppingCartService>();
    }
}
