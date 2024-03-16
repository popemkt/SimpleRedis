using System.Collections.Concurrent;

namespace codecrafters_redis;

public class Cache
{
    private ConcurrentDictionary<string, RedisData> _cache = new();
    
    public void Set(string key, RedisData data)
    =>
        _cache[key] = data;

    public RedisData Get(string key) => _cache[key];
}