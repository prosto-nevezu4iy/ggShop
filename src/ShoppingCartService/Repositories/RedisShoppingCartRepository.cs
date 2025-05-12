namespace ShoppingCartService.Repositories;
using Microsoft.Extensions.Logging;
using ShoppingCartService.Entities;
using StackExchange.Redis;
using System.Text.Json;

public class RedisShoppingCartRepository : IShoppingCartRepository
{
    private readonly ILogger<RedisShoppingCartRepository> _logger;
    private readonly IDatabase _redisDatabase;

    private static RedisKey BasketKeyPrefix = "/basket/";

    private static RedisKey GetShoppingCartKey(string userId) => BasketKeyPrefix.Append(userId);
    private static RedisKey GetUserKey() => BasketKeyPrefix.Append("users");

    public RedisShoppingCartRepository(ILogger<RedisShoppingCartRepository> logger, IConnectionMultiplexer redis)
    {
        _logger = logger;
        _redisDatabase = redis.GetDatabase();
    }

    public async Task<UserShoppingCart> GetBasketAsync(string userId)
    {
        var data = await _redisDatabase.StringGetAsync(GetShoppingCartKey(userId));
        if (data.IsNullOrEmpty)
        {
            return null;
        }

        return JsonSerializer.Deserialize<UserShoppingCart>(data);
    }

    public async Task<UserShoppingCart> UpdateBasketAsync(UserShoppingCart shoppingCart)
    {
        var serializedShoppingCart = JsonSerializer.Serialize(shoppingCart);
        var createdShoppingCart = await _redisDatabase.StringSetAsync(GetShoppingCartKey(shoppingCart.UserId), serializedShoppingCart, TimeSpan.FromDays(30));

        if (!createdShoppingCart)
        {
            _logger.LogInformation("Problem occurred persisting the shopping cart item.");
            return null;
        }
        
        await _redisDatabase.SetAddAsync(GetUserKey(), shoppingCart.UserId);

        _logger.LogInformation("ShoppingCart item persisted successfully.");
        return await GetBasketAsync(shoppingCart.UserId);
    }

    public async Task DeleteBasketAsync(string userId)
    {
        await _redisDatabase.KeyDeleteAsync(GetShoppingCartKey(userId));
        await _redisDatabase.SetRemoveAsync(GetUserKey(), userId);
    }

    public async Task DeleteBasketsByGameIdAsync(Guid gameId)
    {
        var userIds = await _redisDatabase.SetMembersAsync(GetUserKey());
        foreach (var key in userIds)
        {
            var data = await _redisDatabase.StringGetAsync(GetShoppingCartKey(key));
            if (data.IsNullOrEmpty)
            {
                continue;
            }

            var shoppingCart = JsonSerializer.Deserialize<UserShoppingCart>(data);
            
            var removed = shoppingCart.Items.RemoveAll(i => i.GameId == gameId);
            if (removed > 0)
            {
                var updated = JsonSerializer.Serialize(shoppingCart);
                await _redisDatabase.StringSetAsync(GetShoppingCartKey(shoppingCart.UserId), updated, TimeSpan.FromDays(30));
            }
        }
    }
}
