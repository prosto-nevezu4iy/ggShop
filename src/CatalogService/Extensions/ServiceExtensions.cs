using System.Text.Json.Serialization;
using CatalogService.Abstractions;
using CatalogService.Abstractions.Games;
using CatalogService.Abstractions.Genres;
using CatalogService.Abstractions.Platforms;
using CatalogService.Abstractions.Publishers;
using CatalogService.Configurations;
using CatalogService.Entities;
using CatalogService.Enums;
using CatalogService.Infrastructure;
using CatalogService.RequestHelpers;
using CatalogService.Services.Games;
using CatalogService.Services.Genres;
using CatalogService.Services.Platforms;
using CatalogService.Services.Publishers;
using CatalogService.Validators.Games;
using Common.Application.Configurations;
using Common.Infrastructure.Authorization;
using Common.Presentation.Middlewares;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Quartz;
using Scalar.AspNetCore;
using Serilog;
using static Common.Application.Constants.DatabaseConstants;
using static Common.Application.Constants.IdentityConstants;

namespace CatalogService.Extensions;

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
            .AddOptions<CloudinarySettings>()
            .BindConfiguration(CloudinarySettings.Section)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services
            .AddOptions<IdentitySettings>()
            .BindConfiguration(IdentitySettings.Section)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var identitySettings = builder.Configuration.GetSection(IdentitySettings.Section).Get<IdentitySettings>();

        builder.Services.AddDbContext<CatalogContext>(opt =>
        {
            opt.UseNpgsql(builder.Configuration.GetConnectionString(DefaultConnection));
        });

        AddCatalogServices(builder);

        builder.Services.AddValidatorsFromAssemblyContaining<CreateGameDtoValidator>();

        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();

        builder.Services.AddQuartz();

        builder.Services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });

        builder.Services.AddMassTransit(x =>
        {
            x.AddEntityFrameworkOutbox<CatalogContext>(o =>
            {
                o.QueryDelay = TimeSpan.FromSeconds(10);

                o.UsePostgres();
                o.UseBusOutbox();
            });
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);
            });
        });

        builder.Services.AddGrpc();

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = identitySettings.Authority;
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
                options.WithTitle("Catalog Service API");
                options.AddPreferredSecuritySchemes(nameof(SecuritySchemeType.OAuth2));
                options.AddAuthorizationCodeFlow(nameof(SecuritySchemeType.OAuth2), flow =>
                {
                    flow.ClientId = identitySettings.ClientId;
                    flow.ClientSecret = identitySettings.ClientSecret;
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

        app.MapGrpcService<GrpcCatalogService>();
    }

    private static void AddCatalogServices(IHostApplicationBuilder builder)
    {
        // Games
        builder.Services.AddScoped<ISearchBuilder<Game>, GameSearchBuilder>();
        builder.Services.AddScoped<FilterBuilder<Game, GamePagedFilterRequest>, GameFilterBuilder>();
        builder.Services.AddScoped<IOrderBuilder<Game, GameSortOption>, GameOrderBuilder>();

        builder.Services.AddScoped<IGameService, GameService>();
        builder.Services.AddScoped<IImageService, CloudinaryImageService>();
        builder.Services.AddScoped<IJobService, JobService>();
        builder.Services.AddScoped<IUserRatingService, UserRatingService>();

        // Genres
        builder.Services.AddScoped<ISearchBuilder<Genre>, GenreSearchBuilder>();
        builder.Services.AddScoped<IOrderBuilder<Genre, GenreSortOption>, GenreOrderBuilder>();

        builder.Services.AddScoped<IGenreService, GenreService>();

        // Platforms
        builder.Services.AddScoped<ISearchBuilder<Platform>, PlatformSearchBuilder>();
        builder.Services.AddScoped<IOrderBuilder<Platform, PlatformSortOption>, PlatformOrderBuilder>();

        builder.Services.AddScoped<IPlatformService, PlatformService>();

        // Publishers
        builder.Services.AddScoped<ISearchBuilder<Publisher>, PublisherSearchBuilder>();
        builder.Services.AddScoped<IOrderBuilder<Publisher, PublisherSortOption>, PublisherOrderBuilder>();

        builder.Services.AddScoped<IPublisherService, PublisherService>();

        // Permissions
        builder.Services.AddAuthorizationBuilder()
            .AddPolicy(PermissionsList.CatalogCreate, policy => policy.RequirePermission(PermissionsList.CatalogCreate))
            .AddPolicy(PermissionsList.CatalogUpdate, policy => policy.RequirePermission(PermissionsList.CatalogUpdate))
            .AddPolicy(PermissionsList.CatalogDelete, policy => policy.RequirePermission(PermissionsList.CatalogDelete));
    }
}
