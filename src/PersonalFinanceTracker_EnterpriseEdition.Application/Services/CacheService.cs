using Microsoft.Extensions.Caching.Distributed;
using PersonalFinanceTracker_EnterpriseEdition.Application.Abstractions;
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
}
