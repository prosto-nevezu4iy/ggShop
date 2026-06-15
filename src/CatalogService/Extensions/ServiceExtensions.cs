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
using Common.Infrastructure.Authentication;
using Common.Infrastructure.Authorization;
using Common.Infrastructure.Logging;
using Common.Presentation.Extensions;
using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Quartz;
using static Common.Infrastructure.Constants.DatabaseConstants;

namespace CatalogService.Extensions;

public static class ServiceExtensions
{
    public static void AddHostBuilderServices(this IHostBuilder hostBuilder)
    {
        hostBuilder.AddCommonLogging();
    }

    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddCommonControllers();
        builder.Services.AddCommonExceptionHandling();

        var identitySettings = builder.AddCommonIdentitySettings();

        builder.Services
            .AddOptions<CloudinarySettings>()
            .BindConfiguration(CloudinarySettings.Section)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services.AddDbContext<CatalogContext>(opt =>
        {
            opt.UseNpgsql(builder.Configuration.GetConnectionString(DefaultConnection));
        });

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

        builder.Services.AddCommonAuthentication(identitySettings);
        builder.Services.AddCommonAuthorization();

        builder.Services.AddValidatorsFromAssemblyContaining<CreateGameDtoValidator>();
        AddCatalogServices(builder);

        builder.Services.AddGrpc();
        builder.Services.AddCommonSwagger(identitySettings);
    }

    public static void AddMiddlewares(this WebApplication app, IConfiguration configuration)
    {
        var identitySettings = configuration.GetSection(IdentitySettings.Section).Get<IdentitySettings>();
        app.UseCommonScalarUi(identitySettings, "Catalog Service API");

        app.UseCommonMiddlewarePipeline();

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
