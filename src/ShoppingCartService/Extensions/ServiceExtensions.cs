using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Serilog;
using ShoppingCartService.Consumers;
using ShoppingCartService.Repositories;
using StackExchange.Redis;

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
        builder.Services.AddGrpc();

        builder.Services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddSingleton<IShoppingCartRepository, RedisShoppingCartRepository>();

        builder.Services.AddMassTransit(x =>
        {
            x.AddConsumersFromNamespaceContaining<GameDeletedConsumer>();

            x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("shoppingcart", false));

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.ReceiveEndpoint("shoppingcart-game-deleted", e =>
                {
                    e.UseMessageRetry(r => r.Interval(5, 5));

                    e.ConfigureConsumer<GameDeletedConsumer>(context);
                });
                cfg.ReceiveEndpoint("shoppingcart-user-logged-in", e =>
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
                options.Authority = builder.Configuration["IdentityServiceUrl"];
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters.ValidateAudience = false;
                options.TokenValidationParameters.NameClaimType = "username";
            });

        builder.Services.AddAuthorization();
    }
}
