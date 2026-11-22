namespace CoffeeShop.Application.Events;

/// <summary>
/// Інтерфейс шини подій (Event Bus) для відправки доменних та інтеграційних подій.
/// Дозволяє реалізувати слабку зв'язаність між джерелом події та її споживачами.
/// </summary>
public interface IEventBus
{
    Task PublishAsync<TEvent>(TEvent evt, CancellationToken ct = default);
}
