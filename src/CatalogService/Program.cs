using CatalogService.Endpoints;
using CatalogService.Extensions;
using CatalogService.Infrastructure;
using Serilog;

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

    app.UseExceptionHandler();

    //app.UseStatusCodePages();

    // Configure the HTTP request pipeline.
    await CatalogContextSeed.InitDb(app);

    app.MapCatalogApiEndpoints();

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

