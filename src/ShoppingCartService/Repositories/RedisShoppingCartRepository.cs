using ShoppingCartService.Abstractions;
using ShoppingCartService.Entities;
using StackExchange.Redis;
using System.Text.Json;
using Microsoft.Extensions.Options;
using ShoppingCartService.Configurations;
using ShoppingCartService.Helpers;
using static ShoppingCartService.Constants.ShoppingCartConstants;

namespace ShoppingCartService.Repositories;

public class RedisShoppingCartRepository : IShoppingCartRepository
{
    private readonly ILogger<RedisShoppingCartRepository> _logger;
    private readonly IDatabase _redisDatabase;
    private readonly ShoppingCartSettings _shoppingCartSettings;

    private static readonly RedisKey ShoppingCartKeyPrefix = KeyPrefix;

    // This is atomic and fast operation to set multiple keys with expiry using Lua script
    // https://github.com/StackExchange/StackExchange.Redis/issues/1089#issuecomment-471934196
    private static readonly string SetexLuaScript = @"
            local expiry = tonumber(ARGV[1])
            local count = 0
            for i, key in ipairs(KEYS) do
              redis.call('SETEX', key, expiry, ARGV[i+1])
              count = count + 1
            end
            return count";

    private static readonly JsonSerializerOptions JsonSerializerOptions =
        new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, Converters = { new ShoppingCartJsonConverter() }};

    public RedisShoppingCartRepository(
        ILogger<RedisShoppingCartRepository> logger,
        IConnectionMultiplexer redis,
        IOptions<ShoppingCartSettings> options)
    {
        _logger = logger;
        _redisDatabase = redis.GetDatabase();
        _shoppingCartSettings = options.Value;
    }

    public async Task<ShoppingCart> GetShoppingCartAsync(Guid userId)
    {
        var data = await _redisDatabase.StringGetAsync(GetShoppingCartKey(userId));

        return data.IsNullOrEmpty ? new ShoppingCart(userId) : JsonSerializer.Deserialize<ShoppingCart>(data, JsonSerializerOptions);
    }

    public async Task<ShoppingCart> UpdateShoppingCartAsync(ShoppingCart shoppingCart)
    {
        var serializedShoppingCart = JsonSerializer.Serialize(shoppingCart, JsonSerializerOptions);
        var createdShoppingCart = await _redisDatabase.StringSetAsync(GetShoppingCartKey(shoppingCart.UserId), serializedShoppingCart, TimeSpan.FromDays(30));

        if (!createdShoppingCart)
        {
            _logger.LogError("Problem occurred persisting the shopping cart for user {UserId}.", shoppingCart.UserId);
            return null;
        }

        _logger.LogInformation("ShoppingCart item persisted successfully for user {UserId}.", shoppingCart.UserId);

        return await GetShoppingCartAsync(shoppingCart.UserId);
    }

    public async Task<bool> DeleteShoppingCartAsync(Guid userId)
    {
        var shoppingCartDeleted = await _redisDatabase.KeyDeleteAsync(GetShoppingCartKey(userId));

        if (!shoppingCartDeleted)
        {
            _logger.LogError("Problem occurred deleting the shopping cart for user {UserId}.", userId);
            return false;
        }

        _logger.LogInformation("ShoppingCart deleted successfully for user {UserId}.", userId);
        return true;
    }

    public async Task DeleteShoppingCartsByGameIdAsync(Guid gameId)
    {
        var endpoints = _redisDatabase.Multiplexer.GetEndPoints();
        var expirySeconds = (int) TimeSpan.FromDays(_shoppingCartSettings.ExpirationDays).TotalSeconds;
        List<RedisKey> keys = new(_shoppingCartSettings.BatchSize);
        List<RedisValue> values = new(_shoppingCartSettings.BatchSize + 1) { expirySeconds };

        foreach (var endpoint in endpoints)
        {
            var server = _redisDatabase.Multiplexer.GetServer(endpoint);
            var pattern = $"{ShoppingCartKeyPrefix}*";

            await foreach (var key in server.KeysAsync(pattern: pattern))
            {
                var data = await _redisDatabase.StringGetAsync(key);
                if (data.IsNullOrEmpty)
                {
                    continue;
                }

                var shoppingCart = JsonSerializer.Deserialize<ShoppingCart>(data, JsonSerializerOptions);

                var removed = shoppingCart.RemoveItemsByGameId(gameId);

                if (removed > 0)
                {
                    keys.Add(key);
                    values.Add(JsonSerializer.Serialize(shoppingCart, JsonSerializerOptions));
                }

                if (keys.Count == _shoppingCartSettings.BatchSize)
                {
                    await ExecuteBatchAsync(gameId, keys, values);
                    ResetBatch(keys, values, expirySeconds);
                }
            }

            if (keys.Count is not 0)
            {
                await ExecuteBatchAsync(gameId, keys, values);
            }
        }
    }

    public async Task TransferAnonymousCartAsync(Guid anonymousId, Guid userId)
    {
        var anonCart = await GetShoppingCartAsync(anonymousId);
        if (anonCart is null)
        {
            _logger.LogInformation("No anonymous cart for {AnonId}", anonymousId);
            return;
        }

        var userCart = await GetShoppingCartAsync(userId) ?? new ShoppingCart(userId);

        foreach (var item in anonCart.Items)
        {
            var existing = userCart.Items.FirstOrDefault(i => i.GameId == item.GameId);
            if (existing is not null)
            {
                existing.UpdateQuantity(item.Quantity);
            }
            else
            {
                userCart.AddItem(item.GameId, item.Name, item.Price, item.ImageUrl, _shoppingCartSettings);
            }
        }

        await UpdateShoppingCartAsync(userCart);
        await DeleteShoppingCartAsync(anonymousId);
    }

    private static RedisKey GetShoppingCartKey(Guid userId) => ShoppingCartKeyPrefix.Append(userId.ToString());

    private async Task ExecuteBatchAsync(Guid gameId, List<RedisKey> keys, List<RedisValue> values)
    {
        var result = (int) await _redisDatabase.ScriptEvaluateAsync(SetexLuaScript, keys.ToArray(), values.ToArray());
        _logger.LogInformation("Updated {Count} carts - removed game {GameId}", result, gameId);
    }

    private static void ResetBatch(List<RedisKey> keys, List<RedisValue> values, int expirySeconds)
    {
        keys.Clear();
        values.Clear();
        values.Add(expirySeconds);
    }
}
