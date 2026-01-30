namespace ShoppingCartService.DTOs;

public record ShoppingCartDto(IReadOnlyCollection<ShoppingCartItemDto> Items);