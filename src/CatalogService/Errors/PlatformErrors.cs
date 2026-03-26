using Common.Domain;

namespace CatalogService.Errors;

public static class PlatformErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("Catalog.PlatformNotFound", $"Platform with id {id} was not found.");

    public static Error PlatformNotCreated =>
        Error.Problem("Catalog.PlatformNotCreated", "Platform could not be created.");

    public static Error PlatformNotUpdated =>
        Error.Problem("Catalog.PlatformNotUpdated", "Platform could not be updated.");

    public static Error PlatformNotDeleted =>
        Error.Problem("Catalog.PlatformNotDeleted", "Platform could not be deleted.");
}