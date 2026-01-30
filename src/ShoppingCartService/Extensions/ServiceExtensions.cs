using Common.Presentation.Middlewares;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Serilog;
using ShoppingCartService.Abstractions;
using ShoppingCartService.Configurations;
using ShoppingCartService.Consumers;
using ShoppingCartService.Repositories;
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

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = builder.Configuration.GetValue<string>(IdentityServiceUrl);
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters.ValidateAudience = false;
                options.TokenValidationParameters.NameClaimType = IdentityUserName;
            });

        builder.Services.AddAuthorization();
    }

    private static void AddShoppingCartServices(IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IShoppingCartRepository, RedisShoppingCartRepository>();

        builder.Services.AddScoped<IShoppingCartService, Services.ShoppingCartService>();
    }
}
