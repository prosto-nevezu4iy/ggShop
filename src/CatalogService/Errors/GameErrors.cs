using Common.Domain;

namespace CatalogService.Errors;

public static class GameErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("Catalog.GameNotFound", $"Game with id {id} was not found.");

    public static Error GameNotCreated =>
        Error.Problem("Catalog.GameNotCreated", "Game could not be created.");

    public static Error GameNotUpdated =>
        Error.Problem("Catalog.GameNotUpdated", "Game could not be updated.");

    public static Error GameNotDeleted =>
        Error.Problem("Catalog.GameNotDeleted", "Game could not be deleted.");

    public static Error UserRatingNotCreated =>
        Error.Problem("Catalog.UserRatingNotCreated", "User Rating could not be created.");

    public static Error UserRatingNotUpdated =>
        Error.Problem("Catalog.UserRatingNotUpdated", "User Rating could not be updated.");

    public static Error UserRatingNotDeleted =>
        Error.Problem("Catalog.UserRatingNotDeleted", "User Rating could not be deleted.");

    public static Error UserRatingNotFound =>
        Error.NotFound("Catalog.UserRatingNotFound", "User Rating was not found.");
}