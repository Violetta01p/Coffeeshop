using CoffeeShop.Application.Common.Time;

namespace CoffeeShop.Infrastructure.Services;

/// <summary>
/// Реалізація IDateTimeProvider через стандартний DateTime.UtcNow (Singleton).
/// </summary>
public class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
