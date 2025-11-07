using OrderService.Api.Endpoints;
using OrderService.Api.Extensions;
using Serilog;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.AddHostBuilderServices();
    builder.AddApplicationServices();

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    app.MapOrdersApiEndpoints()
        .RequireAuthorization();

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}