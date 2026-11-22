namespace CoffeeShop.Application.Events;

/// <summary>
/// Інтерфейс обробника конкретного типу події TEvent.
/// </summary>
public interface IEventHandler<in TEvent>
{
    Task HandleAsync(TEvent evt, CancellationToken ct = default);
}
