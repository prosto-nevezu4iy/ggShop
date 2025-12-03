namespace CatalogService.DTOs;

public record UserRatingDto(Guid Id, Guid GameId, byte Rating);