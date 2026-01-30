using Common.Domain;

namespace ShoppingCartService.Errors;

public class ShoppingCartErrors
{
    public static Error ShoppingCartNotUpdated =>
        Error.Problem("ShoppingCart.ShoppingCartNotUpdated", "ShoppingCart could not be updated.");

    public static Error ShoppingCartNotDeleted =>
        Error.Problem("ShoppingCart.ShoppingCartNotDeleted", "ShoppingCart could not be deleted.");

    public static Error NotFound(Guid id) =>
        Error.NotFound("ShoppingCart.ShoppingCartNotFound", $"ShoppingCart with user id {id} was not found.");

    public static Error ItemNotFound(Guid id) =>
        Error.NotFound("ShoppingCart.ItemNotFound", $"Item with game id {id} was not found in the shopping cart.");

    public static Error InvalidQuantity(Guid gameId, int quantity) =>
        Error.Problem("ShoppingCart.InvalidQuantity", $"The quantity {quantity} for game id {gameId} is invalid. It must be 1 or 2.");
}