using System.Runtime.Caching;

namespace codecrafters_redis;

public class Cache
{
    private readonly Lazy<MemoryCache> _cache = new(() => MemoryCache.Default);

    public void Set(string key, RedisData data, TimeSpan? expiration = null)
    {
        var options = new CacheItemPolicy();
        if (expiration != null)
            options.SlidingExpiration = expiration.Value;

        _cache.Value.Set(key, data, options);
    }

    public RedisData? Get(string key)
    {
        if (_cache.Value.Contains(key))
           return _cache.Value.Get(key) as RedisData;
        
        return null;
    }
}
