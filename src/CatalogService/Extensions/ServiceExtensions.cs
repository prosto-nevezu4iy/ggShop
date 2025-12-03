using CatalogService.Abstractions;
using CatalogService.Configurations;
using CatalogService.Entities;
using CatalogService.Infrastructure;
using CatalogService.RequestHelpers;
using CatalogService.Services;
using CatalogService.Validators;
using Common.Presentation.Middlewares;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Serilog;

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
            .AddOptions<CloudinarySettings>()
            .BindConfiguration(nameof(CloudinarySettings))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services.AddDbContext<CatalogContext>(opt =>
        {
            opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
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
                options.Authority = builder.Configuration.GetValue<string>("IdentityServiceUrl");
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters.ValidateAudience = false;
                options.TokenValidationParameters.NameClaimType = "username";
            });

        builder.Services.AddAuthorization();
    }

    private static void AddCatalogServices(IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<ISearchBuilder<Game>, GameSearchBuilder>();
        builder.Services.AddScoped<FilterBuilder<Game, GamePagedFilterRequest>, GameFilterBuilder>();
        builder.Services.AddScoped<IOrderBuilder<Game>, GameOrderBuilder>();

        builder.Services.AddScoped<IGameService, Services.GameService>();
        builder.Services.AddScoped<IImageService, CloudinaryImageService>();
        builder.Services.AddScoped<IJobService, JobService>();
        builder.Services.AddScoped<IUserRatingService, UserRatingService>();
    }
}
