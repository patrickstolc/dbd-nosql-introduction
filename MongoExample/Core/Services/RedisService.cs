using StackExchange.Redis;
using System.Text.Json;

namespace MongoExample.Core.Services;

public interface IRedisService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
    Task DeleteAsync(string key);
    Task<bool> CheckRateLimitAsync(string key, int maxAttempts, TimeSpan timeWindow);
}

public class RedisService : IRedisService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _db;

    public RedisService(string connectionString)
    {
        _redis = ConnectionMultiplexer.Connect(connectionString);
        _db = _redis.GetDatabase();
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var value = await _db.StringGetAsync(key);
        return value.HasValue ? JsonSerializer.Deserialize<T>(value!) : default;
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var serializedValue = JsonSerializer.Serialize(value);
        await _db.StringSetAsync(key, serializedValue, expiry);
    }

    public async Task DeleteAsync(string key)
    {
        await _db.KeyDeleteAsync(key);
    }

    public async Task<bool> CheckRateLimitAsync(string key, int maxAttempts, TimeSpan timeWindow)
    {
        var value = await _db.StringIncrementAsync(key);
        if (value == 1)
        {
            await _db.KeyExpireAsync(key, timeWindow);
        }
        return value <= maxAttempts;
    }
} 