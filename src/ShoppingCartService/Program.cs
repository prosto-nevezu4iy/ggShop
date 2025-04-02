using Serilog;
using ShoppingCartService.Extensions;
using ShoppingCartService.Services;

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Host.AddHostBuilderServices();
    builder.AddApplicationServices();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    app.MapGrpcService<GrpcShoppingCartService>();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
