namespace ShoppingCartService.DTOs;

public record ShoppingCartItemDto(Guid GameId, string Name, decimal Price, int Quantity, string ImageUrl);