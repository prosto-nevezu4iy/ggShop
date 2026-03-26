using Common.Domain;

namespace CatalogService.Errors;

public static class GenreErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("Catalog.GenreNotFound", $"Genre with id {id} was not found.");

    public static Error GenreNotCreated =>
        Error.Problem("Catalog.GenreNotCreated", "Genre could not be created.");

    public static Error GenreNotUpdated =>
        Error.Problem("Catalog.GenreNotUpdated", "Genre could not be updated.");

    public static Error GenreNotDeleted =>
        Error.Problem("Catalog.GenreNotDeleted", "Genre could not be deleted.");
}