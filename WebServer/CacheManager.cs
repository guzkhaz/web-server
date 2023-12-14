using Microsoft.Extensions.Caching.Memory;

namespace WebServer;

public static class CacheManager
{
   private static MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
    public static T Get<T>(string key)
    {
        return (T)_cache.Get(key);
    }

    public static void Set<T>(string key, T value)
    {
        _cache.Set(key, value);
    }

    public static void Remove(string key)
    {
        _cache.Remove(key);
    }
}