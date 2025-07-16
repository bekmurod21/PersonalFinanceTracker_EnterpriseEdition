using Microsoft.Extensions.Caching.Distributed;
using PersonalFinanceTracker_EnterpriseEdition.Application.Abstractions;
using StackExchange.Redis;
using System.Text.Json;

namespace PersonalFinanceTracker_EnterpriseEdition.Application.Services;

public class CacheService(IDistributedCache cache) : ICacheService
{
    public async Task CreateCacheAsync(string key, object information)
    {
        try
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes(information);
            await cache.SetAsync(key, bytes, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(5) });
        }
        catch
        {
            await Task.CompletedTask;
        }
    }

    public async Task<T> GetCacheAsync<T>(string key)
    {
        try
        {
            var cachedBytes = await cache.GetAsync(key);
            if (cachedBytes != null)
                return JsonSerializer.Deserialize<T>(cachedBytes)!;
            return default;
        }
        catch
        {
            return default;
        }
    }
    public async Task DeleteAsync(string key)
    {
        try
        {
            var redis = ConnectionMultiplexer.Connect("redis");
            var server = redis.GetServer("redis:6379");
            var keys = server.KeysAsync(pattern: "*").ToBlockingEnumerable();
            var deletedKeys = keys.Where(k => k.ToString().Contains(key));
            foreach (var k in deletedKeys)
                await cache?.RemoveAsync(k);
        }
        catch
        {
            await Task.CompletedTask;
        }
    }
}
