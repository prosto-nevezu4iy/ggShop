using Common.Domain;
using ShoppingCartService.DTOs;

namespace ShoppingCartService.Abstractions;

public interface IShoppingCartService
{
    Task<ShoppingCartDto> GetShoppingCartAsync(Guid userId);
    Task<Result<ShoppingCartDto>> AddItemToShoppingCartAsync(Guid userId, CreateShoppingCartItemDto dto);
    Task<Result<ShoppingCartDto>> UpdateQuantityAsync(Guid userId, Guid gameId, UpdateShoppingCartItemDto dto);
    Task<Result> DeleteShoppingCartAsync(Guid userId);
}