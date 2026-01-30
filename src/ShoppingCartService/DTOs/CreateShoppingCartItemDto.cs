namespace ShoppingCartService.DTOs;

public record CreateShoppingCartItemDto(Guid GameId, string Name, decimal Price, string ImageUrl);
