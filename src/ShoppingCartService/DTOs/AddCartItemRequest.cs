namespace ShoppingCartService.DTOs;

public record AddCartItemRequest(Guid GameId, string Name, decimal Price, int Quantity, string ImageUrl);
