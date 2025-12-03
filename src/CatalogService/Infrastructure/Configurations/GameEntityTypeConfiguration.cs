using CatalogService.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.Infrastructure.Configurations;

public class GameEntityTypeConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.Property(g => g.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(g => g.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(g => g.FullDescription)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(g => g.SystemRequirements)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(g => g.ImageUrl)
            .IsRequired()
            .HasMaxLength(2048);

        builder.Property(g => g.TrailerUrl)
            .IsRequired()
            .HasMaxLength(2048);

        builder.Property(g => g.BackgroundUrl)
            .IsRequired()
            .HasMaxLength(2048);

        builder.Property(g => g.ScreenShotUrls)
            .IsRequired();

        builder.Property(g => g.DiscountPrice)
            .HasComputedColumnSql(@"""Price"" - (""Price"" * ""Discount"" / 100)", stored: true);

        builder.HasMany(g => g.Genres)
            .WithMany(g => g.Games)
            .UsingEntity("GamesGenres");

        builder.HasMany(g => g.Platforms)
            .WithMany(g => g.Games)
            .UsingEntity("GamesPlatforms");

        builder.Property(g => g.SearchVector)
            .IsRequired();

        builder.HasGeneratedTsVectorColumn(g => g.SearchVector, "english", g => new { g.Name, g.Description })
            .HasIndex(g => g.SearchVector)
            .HasMethod("GIN");

        builder.Ignore(g => g.AverageUserRating);
        builder.Ignore(g => g.TotalUserRatings);
    }
}