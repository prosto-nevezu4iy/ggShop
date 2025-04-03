namespace ShoppingCartService.Repositories;
using Microsoft.Extensions.Logging;
using ShoppingCartService.Entities;
using StackExchange.Redis;
using System.Text.Json;

public class RedisShoppingCartRepository : IShoppingCartRepository
{
    private readonly ILogger<RedisShoppingCartRepository> _logger;
    private readonly IDatabase _redis;

    private static RedisKey BasketKeyPrefix = "/basket/";

    private static RedisKey GetShoppingCartKey(string userId) => BasketKeyPrefix.Append(userId);

    public RedisShoppingCartRepository(ILogger<RedisShoppingCartRepository> logger, IConnectionMultiplexer redis)
    {
        _logger = logger;
        _redis = redis.GetDatabase();
    }

    public async Task<UserShoppingCart> GetBasketAsync(string userId)
    {
        var data = await _redis.StringGetAsync(GetShoppingCartKey(userId));
        if (data.IsNullOrEmpty)
        {
            return null;
        }

        return JsonSerializer.Deserialize<UserShoppingCart>(data);
    }

    public async Task<UserShoppingCart> UpdateBasketAsync(UserShoppingCart shoppingCart)
    {
        var serializedShoppingCart = JsonSerializer.Serialize(shoppingCart);
        var createdShoppingCart = await _redis.StringSetAsync(GetShoppingCartKey(shoppingCart.UserId), serializedShoppingCart, TimeSpan.FromDays(30));

        if (!createdShoppingCart)
        {
            _logger.LogInformation("Problem occurred persisting the shopping cart item.");
            return null;
        }

        _logger.LogInformation("ShoppingCart item persisted successfully.");
        return await GetBasketAsync(shoppingCart.UserId);
    }

    public async Task<bool> DeleteBasketAsync(string id)
    {
        return await _redis.KeyDeleteAsync(GetShoppingCartKey(id));
    }
}
