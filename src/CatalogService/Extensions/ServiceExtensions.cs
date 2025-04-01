using CatalogService.Configurations;
using CatalogService.Infrastructure;
using CatalogService.Services;
using CatalogService.Validators;
using CloudinaryDotNet;
using FluentValidation;
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

        builder.Services.AddProblemDetails();

        builder.Services.AddQuartz();

        builder.Services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });
    }
}
