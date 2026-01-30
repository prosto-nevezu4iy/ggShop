using Common.Domain;
using Microsoft.Extensions.Options;
using ShoppingCartService.Abstractions;
using ShoppingCartService.Configurations;
using ShoppingCartService.DTOs;
using ShoppingCartService.Errors;
using ShoppingCartService.Extensions;

namespace ShoppingCartService.Services;

public class ShoppingCartService : IShoppingCartService
{
    private readonly IShoppingCartRepository _shoppingCartRepository;
    private readonly ILogger<ShoppingCartService> _logger;
    private readonly ShoppingCartSettings _shoppingCartSettings;

    public ShoppingCartService(
        IShoppingCartRepository shoppingCartRepository,
        ILogger<ShoppingCartService> logger,
        IOptions<ShoppingCartSettings> options)
    {
        _shoppingCartRepository = shoppingCartRepository;
        _logger = logger;
        _shoppingCartSettings = options.Value;
    }

    public async Task<ShoppingCartDto> GetShoppingCartAsync(Guid userId)
    {
        _logger.LogDebug("Begin GetShoppingCart call from method {Method} for basket id {Id}", nameof(GetShoppingCartAsync), userId);

        var data = await _shoppingCartRepository.GetShoppingCartAsync(userId);

        return data.ToDto();
    }

    public async Task<Result<ShoppingCartDto>> AddItemToShoppingCartAsync(Guid userId, CreateShoppingCartItemDto dto)
    {
        var cart = await _shoppingCartRepository.GetShoppingCartAsync(userId);
        if (cart is null)
        {
            return ShoppingCartErrors.NotFound(userId);
        }

        var result = cart.AddItem(dto.GameId, dto.Name, dto.Price, dto.ImageUrl, _shoppingCartSettings);

        if (result.IsFailure)
        {
            return result.Error;
        }

        var response = await _shoppingCartRepository.UpdateShoppingCartAsync(cart);

        return response is null ? ShoppingCartErrors.ShoppingCartNotUpdated : response.ToDto();
    }

    public async Task<Result<ShoppingCartDto>> UpdateQuantityAsync(Guid userId, Guid gameId, UpdateShoppingCartItemDto dto)
    {
        var cart = await _shoppingCartRepository.GetShoppingCartAsync(userId);
        if (cart is null)
        {
            return ShoppingCartErrors.NotFound(userId);
        }

        var result = cart.UpdateItemQuantity(gameId, dto.Quantity, _shoppingCartSettings);

        if (result.IsFailure)
        {
            return result.Error;
        }

        var updatedCart = await _shoppingCartRepository.UpdateShoppingCartAsync(cart);

        return updatedCart is null ? ShoppingCartErrors.ShoppingCartNotUpdated : updatedCart.ToDto();
    }

    public async Task<Result> DeleteShoppingCartAsync(Guid userId)
    {
        var result = await _shoppingCartRepository.DeleteShoppingCartAsync(userId);

        return result ? Result.Success() : ShoppingCartErrors.ShoppingCartNotDeleted;
    }
}