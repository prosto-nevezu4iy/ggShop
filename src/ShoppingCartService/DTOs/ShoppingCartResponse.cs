namespace ShoppingCartService.DTOs;

public record ShoppingCartResponse(List<ShoppingCartItemDto> Items)
{
    public ShoppingCartResponse() : this(new List<ShoppingCartItemDto>())
    {

    }
}