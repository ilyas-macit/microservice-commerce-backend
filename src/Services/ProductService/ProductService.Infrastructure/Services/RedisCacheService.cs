using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using ProductService.Application.Interfaces;

namespace ProductService.Infrastructure.Services;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _distributedCache;

    public RedisCacheService(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            var json = await _distributedCache.GetStringAsync(key);
            if (string.IsNullOrWhiteSpace(json))
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(json);
        }
        catch
        {
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan expiry)
    {
        try
        {
            var json = JsonSerializer.Serialize(value);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiry
            };

            await _distributedCache.SetStringAsync(key, json, options);
        }
        catch
        {
            // Redis unavailable: cache write failure should not break request flow.
        }
    }

    public async Task RemoveAsync(string key)
    {
        try
        {
            await _distributedCache.RemoveAsync(key);
        }
        catch
        {
            // Redis unavailable: cache invalidation failure should not break request flow.
        }
    }
}
