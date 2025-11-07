using CatalogService.Configurations;
using CatalogService.Infrastructure;
using CatalogService.Middlewares;
using CatalogService.Services;
using CatalogService.Validators;
using CloudinaryDotNet;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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
        builder.Services.AddGrpc();

        builder.Services.AddSingleton<IValidateOptions<CloudinarySettings>, CloudinarySettingsValidation>();
        builder.Services.AddOptionsWithValidateOnStart<CloudinarySettings>()
            .Bind(builder.Configuration.GetRequiredSection(nameof(CloudinarySettings)));

        builder.Services.AddDbContext<CatalogContext>(opt =>
        {
            opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
        });

        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        builder.Services.AddScoped<ICatalogService, Services.CatalogService>();
        builder.Services.AddScoped<IImageService, CloudinaryImageService>();


        var cloudinarySettings = builder.Configuration.GetRequiredSection(nameof(CloudinarySettings)).Get<CloudinarySettings>();
        builder.Services.AddSingleton(new Cloudinary(new Account(cloudinarySettings.CloudName, cloudinarySettings.ApiKey, cloudinarySettings.ApiSecret)));

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

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = builder.Configuration["IdentityServiceUrl"];
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters.ValidateAudience = false;
                options.TokenValidationParameters.NameClaimType = "username";
            });

        builder.Services.AddAuthorization();
    }
}
