using CatalogService.Endpoints;
using CatalogService.Extensions;
using CatalogService.Infrastructure;
using CatalogService.Services;
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

    // Configure the HTTP request pipeline.
    await CatalogContextSeed.InitDb(app);

    app.MapGamesApiEndpoints();
    app.MapGrpcService<GrpcCatalogService>();

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
