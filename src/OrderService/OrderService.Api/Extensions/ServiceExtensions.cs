using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using OrderService.Application.Extensions;
using OrderService.Application.Services;
using OrderService.Infrastructure;
using Serilog;

namespace OrderService.Api.Extensions;

public static class ServiceExtensions
{
    public static void AddHostBuilderServices(this IHostBuilder hostBuilder)
    {
        hostBuilder.UseSerilog((context, loggerConfiguration) =>
            loggerConfiguration.ReadFrom.Configuration(context.Configuration));
    }

    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDbContext<OrderingContext>(opt =>
        {
            opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
        });

        builder.Services.AddApplicationServices();

        builder.Services.AddScoped<GrpcShoppingCartClient>();
        builder.Services.AddScoped<GrpcCatalogClient>();

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