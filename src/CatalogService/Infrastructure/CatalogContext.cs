using CatalogService.Entities;
using CatalogService.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Infrastructure;

public class CatalogContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Game> Games { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Publisher> Publishers { get; set; }
    public DbSet<Platform> Platforms { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new GameEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new GenreEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new PlatformEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new GenreEntityTypeConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}
