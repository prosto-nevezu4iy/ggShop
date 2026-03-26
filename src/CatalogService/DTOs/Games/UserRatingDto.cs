namespace CatalogService.DTOs.Games;

public record UserRatingDto(Guid Id, Guid GameId, byte Rating);