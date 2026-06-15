using Common.Infrastructure.Authentication;
using Common.Infrastructure.Authorization;
using Common.Presentation.Extensions;
using MassTransit;
using Serilog;
using ShoppingCartService.Abstractions;
using ShoppingCartService.Configurations;
using ShoppingCartService.Consumers;
using ShoppingCartService.Repositories;
using ShoppingCartService.Services;
using StackExchange.Redis;
using static Common.Infrastructure.Constants.DatabaseConstants;
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
        builder.Services.AddCommonControllers();
        builder.Services.AddCommonExceptionHandling();
        
        var identitySettings = builder.AddCommonIdentitySettings();
        
        builder.Services
            .AddOptions<ShoppingCartSettings>()
            .BindConfiguration(ShoppingCartSettings.Section)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString(DefaultConnection) ??
                                          throw new InvalidOperationException()));
        
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
        
        builder.Services.AddCommonAuthentication(identitySettings);
        builder.Services.AddCommonAuthorization();
        
        AddShoppingCartServices(builder);

        builder.Services.AddGrpc();
        
        builder.Services.AddCommonSwagger(identitySettings);
    }
    
    public static void AddMiddlewares(this WebApplication app, IConfiguration configuration)
    {
        var identitySettings = configuration.GetSection(IdentitySettings.Section).Get<IdentitySettings>();
        app.UseCommonScalarUi(identitySettings, "Shopping Cart Service API");

        app.UseCommonMiddlewarePipeline();

        app.MapGrpcService<GrpcShoppingCartService>();
    }

    private static void AddShoppingCartServices(IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IShoppingCartRepository, RedisShoppingCartRepository>();

        builder.Services.AddScoped<IShoppingCartService, Services.ShoppingCartService>();
    }
}
