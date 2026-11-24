using System.Text.Json;
using CoffeeShop.Application.Common.Caching;
using Enyim.Caching;
using Microsoft.Extensions.Logging;

namespace CoffeeShop.Infrastructure.Caching;

/// <summary>
/// Реалізація розподіленого кешування на базі Memcached з відмовостійкістю (resilience).
/// При недоступності сервера Memcached бізнес-запити не падають, а логується Warning та виконується graceful fallback.
/// </summary>
public class MemcachedCacheService : ICacheService
{
    private readonly IMemcachedClient _memcachedClient;
    private readonly ILogger<MemcachedCacheService> _logger;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public MemcachedCacheService(
        IMemcachedClient memcachedClient,
        ILogger<MemcachedCacheService> logger)
    {
        _memcachedClient = memcachedClient;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        try
        {
            var result = await _memcachedClient.GetAsync<string>(key);
            if (result != null && result.Success && !string.IsNullOrEmpty(result.Value))
            {
                _logger.LogInformation("[CACHE HIT] key: {Key}", key);
                return JsonSerializer.Deserialize<T>(result.Value, _jsonOptions);
            }

            _logger.LogInformation("[CACHE MISS] key: {Key}", key);
            return default;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "[CACHE WARNING] Сервер Memcached недоступний при отриманні ключа '{Key}'. Виконується graceful fallback до сховища.", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken ct = default)
    {
        try
        {
            string json = JsonSerializer.Serialize(value, _jsonOptions);
            int validForSeconds = (int)Math.Max(1, ttl.TotalSeconds);
            await _memcachedClient.SetAsync(key, json, validForSeconds);
            _logger.LogInformation("[CACHE SET] key: {Key}, TTL: {TTL}s", key, validForSeconds);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "[CACHE WARNING] Сервер Memcached недоступний при збереженні ключа '{Key}'. Продовження без кешу.", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken ct = default)
    {
        try
        {
            await _memcachedClient.RemoveAsync(key);
            _logger.LogInformation("[CACHE INVALIDATE] key: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "[CACHE WARNING] Сервер Memcached недоступний при видаленні ключа '{Key}'. Продовження без інвалідації.", key);
        }
    }
}
