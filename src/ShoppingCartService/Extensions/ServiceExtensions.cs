using MassTransit;
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
                cfg.ConfigureEndpoints(context);
            });
        });
    }
}
