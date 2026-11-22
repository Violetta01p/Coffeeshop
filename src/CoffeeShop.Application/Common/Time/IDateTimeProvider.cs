namespace CoffeeShop.Application.Common.Time;

/// <summary>
/// Абстракція часу (DIP / IoC).
/// Дозволяє тестувати компоненти без статичної прив'язки до системного годинника DateTime.UtcNow.
/// </summary>
public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
