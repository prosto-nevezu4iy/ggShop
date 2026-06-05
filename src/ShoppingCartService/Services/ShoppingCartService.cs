using Common.Domain;
using Microsoft.Extensions.Options;
using ShoppingCartService.Abstractions;
using ShoppingCartService.Configurations;
using ShoppingCartService.DTOs;
using ShoppingCartService.Errors;
using ShoppingCartService.Extensions;

namespace ShoppingCartService.Services;

public class ShoppingCartService(
    IShoppingCartRepository shoppingCartRepository,
    ILogger<ShoppingCartService> logger,
    IOptions<ShoppingCartSettings> options)
    : IShoppingCartService
{
    private readonly ShoppingCartSettings _shoppingCartSettings = options.Value;

    public async Task<ShoppingCartDto> GetShoppingCartAsync(Guid userId)
    {
        logger.LogDebug("Begin GetShoppingCart call from method {Method} for basket id {Id}", nameof(GetShoppingCartAsync), userId);

        var data = await shoppingCartRepository.GetShoppingCartAsync(userId);

        return data.ToDto();
    }

    public async Task<Result<ShoppingCartDto>> AddItemToShoppingCartAsync(Guid userId, CreateShoppingCartItemDto dto)
    {
        var cart = await shoppingCartRepository.GetShoppingCartAsync(userId);
        if (cart is null)
        {
            return ShoppingCartErrors.NotFound(userId);
        }

        var result = cart.AddItem(dto.GameId, dto.Name, dto.Price, dto.ImageUrl, _shoppingCartSettings);

        if (result.IsFailure)
        {
            return result.Error;
        }

        var response = await shoppingCartRepository.UpdateShoppingCartAsync(cart);

        return response is null ? ShoppingCartErrors.ShoppingCartNotUpdated : response.ToDto();
    }

    public async Task<Result<ShoppingCartDto>> UpdateQuantityAsync(Guid userId, Guid gameId, UpdateShoppingCartItemDto dto)
    {
        var cart = await shoppingCartRepository.GetShoppingCartAsync(userId);
        if (cart is null)
        {
            return ShoppingCartErrors.NotFound(userId);
        }

        var result = cart.UpdateItemQuantity(gameId, dto.Quantity, _shoppingCartSettings);

        if (result.IsFailure)
        {
            return result.Error;
        }

        var updatedCart = await shoppingCartRepository.UpdateShoppingCartAsync(cart);

        return updatedCart is null ? ShoppingCartErrors.ShoppingCartNotUpdated : updatedCart.ToDto();
    }

    public async Task<Result> DeleteShoppingCartAsync(Guid userId)
    {
        var result = await shoppingCartRepository.DeleteShoppingCartAsync(userId);

        return result ? Result.Success() : ShoppingCartErrors.ShoppingCartNotDeleted;
    }
}