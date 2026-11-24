namespace CoffeeShop.Application.Common.Caching;

/// <summary>
/// Абстракція сервісу розподіленого або локального кешування.
/// Дозволяє реалізувати патерн Cache-Aside без прив'язки шару Application до конкретної бібліотеки (Memcached, Redis тощо).
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Отримати значення з кешу за ключем.
    /// </summary>
    Task<T?> GetAsync<T>(string key, CancellationToken ct = default);

    /// <summary>
    /// Зберегти значення в кеші із заданим часом життя (TTL).
    /// </summary>
    Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken ct = default);

    /// <summary>
    /// Видалити (інвалідувати) значення з кешу за ключем.
    /// </summary>
    Task RemoveAsync(string key, CancellationToken ct = default);
}
