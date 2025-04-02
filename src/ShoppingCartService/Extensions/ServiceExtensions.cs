using Serilog;
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
    }
}
