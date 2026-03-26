using Common.Domain;

namespace CatalogService.Errors;

public class PublisherErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("Catalog.PublisherNotFound", $"Publisher with id {id} was not found.");

    public static Error PublisherNotCreated =>
        Error.Problem("Catalog.PublisherNotCreated", "Publisher could not be created.");

    public static Error PublisherNotUpdated =>
        Error.Problem("Catalog.PublisherNotUpdated", "Publisher could not be updated.");

    public static Error PublisherNotDeleted =>
        Error.Problem("Catalog.PublisherNotDeleted", "Publisher could not be deleted.");
}