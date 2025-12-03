using CatalogService.DTOs;
using CatalogService.Entities;

namespace CatalogService.Extensions;

public static class UserRatingExtensions
{
    public static UserRatingDto ToDto(this UserRating userRating)
    {
        return new UserRatingDto(
            userRating.Id,
            userRating.GameId,
            userRating.Rating);
    }
}