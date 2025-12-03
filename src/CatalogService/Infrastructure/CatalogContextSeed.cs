using CatalogService.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CatalogService.Infrastructure;

public class CatalogContextSeed
{
    public static async Task InitDb(WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        await SeedData(scope.ServiceProvider.GetService<CatalogContext>(), app.Environment, scope.ServiceProvider.GetService<ILogger<CatalogContextSeed>>());
    }

    private static async Task SeedData(CatalogContext context, IWebHostEnvironment env, ILogger<CatalogContextSeed> logger)
    {
        await context.Database.MigrateAsync();

        var contentRootPath = env.ContentRootPath;

        if (!context.Genres.Any())
        {
            var sourcePath = Path.Combine(contentRootPath, "Setup", "genres.json");
            var sourceJson = await File.ReadAllTextAsync(sourcePath);
            var sourceItems = JsonSerializer.Deserialize<List<Genre>>(sourceJson);

            await context.Genres.AddRangeAsync(sourceItems);
            await context.SaveChangesAsync();
            logger.LogInformation("Seeded catalog with {NumGenres} genres", context.Genres.Count());
        }

        if (!context.Publishers.Any())
        {
            var sourcePath = Path.Combine(contentRootPath, "Setup", "publishers.json");
            var sourceJson = await File.ReadAllTextAsync(sourcePath);
            var sourceItems = JsonSerializer.Deserialize<List<Publisher>>(sourceJson);

            await context.Publishers.AddRangeAsync(sourceItems);
            await context.SaveChangesAsync();
            logger.LogInformation("Seeded catalog with {NumPublishers} publishers", context.Publishers.Count());
        }

        if (!context.Platforms.Any())
        {
            var sourcePath = Path.Combine(contentRootPath, "Setup", "platforms.json");
            var sourceJson = await File.ReadAllTextAsync(sourcePath);
            var sourceItems = JsonSerializer.Deserialize<List<Platform>>(sourceJson);

            await context.Platforms.AddRangeAsync(sourceItems);
            await context.SaveChangesAsync();
            logger.LogInformation("Seeded catalog with {NumPlatforms} platforms", context.Platforms.Count());
        }

        if (!context.Games.Any())
        {
            var sourcePath = Path.Combine(contentRootPath, "Setup", "games.json");
            var sourceJson = await File.ReadAllTextAsync(sourcePath);
            var sourceItems = JsonSerializer.Deserialize<List<Game>>(sourceJson);

            var games = sourceItems.Select(s => new Game
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                FullDescription = s.FullDescription,
                SystemRequirements = s.SystemRequirements,
                Price = s.Price,
                Discount = s.Discount,
                AvailableStock = s.AvailableStock,
                Platforms = context.Platforms.Where(pf => s.Platforms.Any(x => x.Name == pf.Name)).ToList(),
                Publisher = context.Publishers.FirstOrDefault(x => x.Name == s.Publisher.Name),
                ReleaseDate = s.ReleaseDate,
                Rating = s.Rating,
                ImageUrl = s.ImageUrl,
                TrailerUrl = s.TrailerUrl,
                BackgroundUrl = s.BackgroundUrl,
                ScreenShotUrls = s.ScreenShotUrls,
                Genres = context.Genres.Where(gn => s.Genres.Any(x => x.Name == gn.Name)).ToList()
            }).ToList();

            await context.Games.AddRangeAsync(games);
            await context.SaveChangesAsync();
            logger.LogInformation("Seeded catalog with {NumGames} games", context.Games.Count());
        }
    }
}
