using Serilog;
using ShoppingCartService.Endpoints;
using ShoppingCartService.Extensions;
using ShoppingCartService.Services;

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Host.AddHostBuilderServices();
    builder.AddApplicationServices();

    var app = builder.Build();

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseSerilogRequestLogging();

    // app.UseExceptionHandler();

    // Configure the HTTP request pipeline.
    app.MapShoppingCartApiEndpoints();
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
