using CatalogService.Extensions;
using CatalogService.Infrastructure;
using CatalogService.Services.Games;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Serilog;

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Host.AddHostBuilderServices();
    builder.AddApplicationServices();

    var app = builder.Build();

    app.AddMiddlewares(builder.Configuration);

    await CatalogContextSeed.InitDb(app);

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
